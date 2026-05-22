using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Utilities;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;

namespace ZucoHR.Application.Services
{
    public class RecruitmentService : IRecruitmentService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IRecruitmentRepository _repository;
        private readonly ITenantService _tenantService;
        private readonly ZucoHrDbContext _context;
        private readonly IEmailService _emailService;
        
        public RecruitmentService(IEmailService emailService,IWebHostEnvironment env,ZucoHrDbContext context, IRecruitmentRepository repository, ITenantService tenantService)
        {
            _repository = repository;
            _tenantService = tenantService;
            _context = context;
            _emailService = emailService;
            _env = env;
        }

        // Jobs
        public async Task<PaginatedResponse<JobPost>> GetJobs(int page = 1, int pageSize= 20)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetJobsAsync(orgId, page, pageSize);
        }

        public async Task<JobPost?> GetJob(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetJobByIdAsync(orgId, id);
        }

        public async Task CreateJob(JobPostRequest request)
        {

            JobPost job = new JobPost();

            job.Id = Guid.NewGuid();

            job.OrganizationId = _tenantService.GetTenantId();

            var org = await _context.Organizations.Where(x => x.Id == job.OrganizationId).FirstOrDefaultAsync();

            job.Organization = org;

            //job.Applicants = request.Applicants;
            job.SalaryMax = request.SalaryMax;
            job.SalaryMin = request.SalaryMin;
            job.Title = request.Title;
            job.Department = request.Department;
            job.Description = request.Description;
            job.Status = request.Status;
            job.Slug = SlugHelper.Generate(request.Title);
            job.Requirements = request.Requirements;
            job.Location = request.Location;
            job.Type = request.Type;
            job.CreatedAt = DateTime.UtcNow;
            job.IsActive = true;

            await _repository.AddJobAsync(job);
        }

        public async Task CloseJobAsync(Guid id)
        {
            var job = await _context.JobPosts.FindAsync(id);

            if (job == null)
                throw new Exception("Job not found");

            job.Status = "Closed";

            await _context.SaveChangesAsync();
        }
        public async Task UpdateJob(Guid id, JobPostRequest updated)
        {
            var orgId = _tenantService.GetTenantId();
            var job = await _repository.GetJobByIdAsync(orgId,id);

            if (job == null)
                throw new Exception("Job not found");

            job.Title = updated.Title;
            job.Description = updated.Description;
            job.Department = updated.Department;
            job.Location = updated.Location;
            job.Status = updated.Status;
            job.Slug = SlugHelper.Generate(updated.Title);
            job.SalaryMax = updated.SalaryMax;
            job.SalaryMin = updated.SalaryMin;
            job.Requirements = updated.Requirements;
            job.Type = updated.Type;
            job.IsActive = true;

            await _repository.UpdateJobAsync(job);
        }

        public async Task DeleteJob(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            var job = await _repository.GetJobByIdAsync(orgId, id);

            if (job == null)
                throw new Exception("Job not found");

            await _repository.DeleteJobAsync(job);
        }

        // Applicants

        public async Task<PaginatedResponse<Applicant>> GetApplicants(int page=1, int pageSize = 100)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetApplicants(orgId, page, pageSize);
        }
        public async Task<List<Applicant>> GetApplicants(Guid jobId)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetApplicantsByJobAsync(orgId, jobId);
        }

        public async Task ApplyAsync(Guid orgId,
    Guid employeeId,
    ApplyJobRequest request)
        {
            // =========================
            // VALIDATE JOB
            // =========================

            var job = await _context.JobPosts
                .FirstOrDefaultAsync(x => x.Id == request.JobId);

            if (job == null)
                throw new Exception("Job not found");

            if (job.Status != "open")
                throw new Exception("This job is no longer accepting applications");

            // =========================
            // CHECK DUPLICATE APPLICATION
            // =========================

            var existing = await _context.Applicants
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.JobPostId == request.JobId);

            if (existing != null)
                throw new Exception(
                    "You already applied for this job");

            // =========================
            // GET EMPLOYEE
            // =========================

            var employee = await _context.Employees
                .FindAsync(employeeId);

            if (employee == null)
                throw new Exception("Employee not found");

            
            // =========================
            // HANDLE RESUME UPLOAD
            // =========================

            string? resumeUrl = null;

            if (request.Resume != null)
            {
                // Ensure wwwroot exists
                if (string.IsNullOrWhiteSpace(_env.WebRootPath))
                {
                    _env.WebRootPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot"
                    );
                }

                // Create resumes folder
                var uploadsFolder = Path.Combine(
                    _env.WebRootPath,
                    "resumes"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Validate extension
                var allowedExtensions = new[]
                {
            ".pdf",
            ".doc",
            ".docx"
        };

                var extension = Path.GetExtension(
                    request.Resume.FileName
                ).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    throw new Exception(
                        "Only PDF or Word documents are allowed"
                    );
                }

                // Validate file size (5MB)
                if (request.Resume.Length > 5 * 1024 * 1024)
                {
                    throw new Exception(
                        "Resume file too large"
                    );
                }

                // Clean filename
                var safeFileName =
                    Path.GetFileNameWithoutExtension(
                        request.Resume.FileName
                    );

                safeFileName = string.Join(
                    "_",
                    safeFileName.Split(
                        Path.GetInvalidFileNameChars()
                    )
                );

                var fileName =
                    $"{Guid.NewGuid()}_{safeFileName}{extension}";

                var filePath = Path.Combine(
                    uploadsFolder,
                    fileName
                );

                // SAVE FILE
                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create
                ))
                {
                    await request.Resume.CopyToAsync(stream);
                }

                resumeUrl = $"/resumes/{fileName}";
            }

            // =========================
            // CREATE APPLICANT
            // =========================

            var candidate = new Applicant
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                
                EmployeeId = employeeId,

                JobPostId = request.JobId,

                FullName =
                    $"{employee.FirstName} {employee.LastName}",

                Email = employee.Email,

                PhoneNumber = employee.PhoneNumber,

                JobPostTitle = job.Title,

                ResumeUrl = resumeUrl,

                CoverLetter = request.CoverLetter,

                Stage = "applied",

                AppliedAt = DateTime.UtcNow
            };

            _context.Applicants.Add(candidate);

            // =========================
            // UPDATE APPLICANT COUNT
            // =========================

            //job.Applicants ??= 0;
            job.Applicants += 1;

            await _context.SaveChangesAsync();
        }
        public async Task UpdateStatus(Guid applicantId, string status)
        {
            var orgId = _tenantService.GetTenantId();
            var applicant = await _repository.GetApplicantByIdAsync(orgId,applicantId);

            if (applicant == null)
                throw new Exception("Applicant not found");

            applicant.Stage = status;

            await _repository.UpdateApplicantAsync(applicant);
        }
    }
}
