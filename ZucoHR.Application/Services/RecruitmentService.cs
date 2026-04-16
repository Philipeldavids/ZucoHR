using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Interfaces;

namespace ZucoHR.Application.Services
{
    public class RecruitmentService : IRecruitmentService
    {
        private readonly IRecruitmentRepository _repository;
        private readonly ITenantService _tenantService;

        public RecruitmentService(IRecruitmentRepository repository, ITenantService tenantService)
        {
            _repository = repository;
            _tenantService = tenantService;
        }

        // Jobs
        public async Task<List<JobPost>> GetJobs()
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetJobsAsync(orgId);
        }

        public async Task<JobPost?> GetJob(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetJobByIdAsync(orgId, id);
        }

        public async Task CreateJob(JobPost job)
        {
            job.Id = Guid.NewGuid();
            job.OrganizationId = _tenantService.GetTenantId();
            job.CreatedAt = DateTime.UtcNow;
            job.IsActive = true;

            await _repository.AddJobAsync(job);
        }

        public async Task UpdateJob(Guid id, JobPost updated)
        {
            var orgId = _tenantService.GetTenantId();
            var job = await _repository.GetJobByIdAsync(orgId,id);

            if (job == null)
                throw new Exception("Job not found");

            job.Title = updated.Title;
            job.Description = updated.Description;
            job.Department = updated.Department;
            job.Location = updated.Location;
            job.IsActive = updated.IsActive;

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
        public async Task<List<Applicant>> GetApplicants(Guid jobId)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetApplicantsByJobAsync(orgId, jobId);
        }

        public async Task Apply(Guid jobId, Applicant applicant)
        {
            applicant.Id = Guid.NewGuid();
            applicant.OrganizationId = _tenantService.GetTenantId();
            applicant.JobPostId = jobId;
            applicant.Status = ApplicationStatus.Applied;
            applicant.AppliedAt = DateTime.UtcNow;

            await _repository.AddApplicantAsync(applicant);
        }

        public async Task UpdateStatus(Guid applicantId, ApplicationStatus status)
        {
            var orgId = _tenantService.GetTenantId();
            var applicant = await _repository.GetApplicantByIdAsync(orgId,applicantId);

            if (applicant == null)
                throw new Exception("Applicant not found");

            applicant.Status = status;

            await _repository.UpdateApplicantAsync(applicant);
        }
    }
}
