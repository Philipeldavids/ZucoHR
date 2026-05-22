using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareersController : ControllerBase
    {
        private readonly ZucoHrDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CareersController(
            ZucoHrDbContext context,
            IWebHostEnvironment env
        )
        {
            _context = context;
            _env = env;
        }

        [HttpGet("{companySlug}/jobs")]
        public async Task<IActionResult> GetJobs(string companySlug)
        {
            var jobs = await _context.JobPosts
                .Where(x => x.Status == "open" && x.Organization.Slug == companySlug)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(jobs);
        }

        [HttpGet("{companySlug}/jobs/{slug}")]
        public async Task<IActionResult> GetJob(string companySlug, string slug)
       {

            var job = await _context.JobPosts
                .FirstOrDefaultAsync(x => x.Slug == slug && x.Organization.Slug == companySlug);

            if (job == null)
                return NotFound();

            return Ok(job);
        }

        [HttpPost("{companySlug}/jobs/{jobId}/apply")]
        public async Task<IActionResult> Apply(string companySlug,
            Guid jobId,
            [FromForm] ApplyJobRequest request
        )
        {
            var job = await _context.JobPosts
                .FirstOrDefaultAsync(x => x.Id == jobId && x.Organization.Slug == companySlug);

            if (job == null)
                return NotFound("Job not found");

            var org = await _context.Organizations.Where(x => x.Slug == companySlug).FirstOrDefaultAsync();
            string? resumePath = null;

            if (request.Resume != null)
            {
                var uploads = Path.Combine(
                    _env.WebRootPath,
                    "uploads/resumes"
                );

                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var fileName =
                    Guid.NewGuid() +
                    Path.GetExtension(request.Resume.FileName);

                var filePath = Path.Combine(uploads, fileName);

                using var stream = new FileStream(
                    filePath,
                    FileMode.Create
                );

                await request.Resume.CopyToAsync(stream);

                resumePath = $"uploads/resumes/{fileName}";
            }

            var application = new Applicant
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                JobPostId = jobId,
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                ResumeUrl = resumePath,
                CoverLetter = request.CoverLetter,
                LinkedinUrl = request.LinkedinUrl,
                PortfolioUrl = request.PortfolioUrl,
                Stage = "applied",
                AppliedAt = DateTime.UtcNow,
            };

            _context.Applicants.Add(application);

            job.Applicants += 1;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Application submitted"
            });
        }
    }
}
