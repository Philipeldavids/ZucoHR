using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecruitmentController : ControllerBase
    {
        private readonly IRecruitmentService _service;
        private readonly ITenantService _tenantService;
        public readonly ZucoHrDbContext _context;
        public RecruitmentController(ZucoHrDbContext context, ITenantService tenantService, IRecruitmentService service)
        {
            _service = service;
            _context = context;
            _tenantService = tenantService;
        }

        // Jobs
        [HttpGet("jobs")]
        [Authorize]
        public async Task<IActionResult> GetJobs()
        {
            int page = 1;
            int pageSize = 20;
            try
            {
                return Ok(await _service.GetJobs(page, pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("jobs")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> CreateJob([FromBody] JobPostRequest job)
        {
            await _service.CreateJob(job);
            return Ok(new { message = "Job created" });
        }

        [HttpPut("jobs/{id}")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> UpdateJob(Guid id, [FromBody] JobPostRequest job)
        {
            await _service.UpdateJob(id, job);
            return Ok(new { message = "Job updated" });
        }

        [HttpDelete("jobs/{id}")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> DeleteJob(Guid id)
        {
            await _service.DeleteJob(id);
            return Ok(new { message = "Job deleted" });
        }
        [HttpPatch("jobs/{id}/close")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> CloseJob(Guid id)
        {
            try
            {
                await _service.CloseJobAsync(id);

                return Ok(new
                {
                    message = "Job closed successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Applicants

        [HttpGet("applicants")]
        [Authorize]
        public async Task<IActionResult> GetApplicants()
        {
            int page = 1;
            int pageSize = 100;

            try
            {
                var res = await _service.GetApplicants(page, pageSize);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("jobs/{jobId}/applicants")]
        [Authorize]
        public async Task<IActionResult> GetApplicants(Guid jobId)
        {
            return Ok(await _service.GetApplicants(jobId));
        }
        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> Apply(
            [FromForm] ApplyJobRequest request)
        {
            try
            {
                var claim = User.FindFirst("employeeId");


                if (claim == null)
                    return Unauthorized();

                var employeeId = Guid.Parse(claim.Value);
                var orgId = _tenantService.GetTenantId();
                await _service.ApplyAsync(
                    orgId,
                    employeeId,
                    request
                );

                return Ok(new
                {
                    message = "Application submitted"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPatch("applicants/{id}/stage")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateApplicantRequest request)
        {
            await _service.UpdateStatus(Guid.Parse(id), request.Status);
            return Ok(new { message = "Status updated" });
        }
    
    [HttpGet("applicants/{candidateId}/cv")]
    [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> DownloadCv(string candidateId)
        {
            var candidate = await _context.Applicants
                .FirstOrDefaultAsync(c => c.Id == Guid.Parse(candidateId));

            if (candidate == null)
                return NotFound("Candidate not found");

            if (string.IsNullOrWhiteSpace(candidate.ResumeUrl))
                return NotFound("CV not uploaded");

            var resumePath = candidate.ResumeUrl
            .TrimStart('/', '\\')
            .Replace("/", Path.DirectorySeparatorChar.ToString())
            .Replace("\\", Path.DirectorySeparatorChar.ToString());

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                resumePath
            );

            if (!System.IO.File.Exists(filePath))
                return NotFound("CV file missing");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            var contentType = "application/octet-stream";

            var extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".pdf")
                contentType = "application/pdf";

            if (extension == ".doc")
                contentType = "application/msword";

            if (extension == ".docx")
                contentType =
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            return File(
                fileBytes,
                contentType,
                Path.GetFileName(filePath)
            );
        }
    }
}