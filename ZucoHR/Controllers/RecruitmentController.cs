using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecruitmentController : ControllerBase
    {
        private readonly IRecruitmentService _service;

        public RecruitmentController(IRecruitmentService service)
        {
            _service = service;
        }

        // Jobs
        [HttpGet("jobs")]
        public async Task<IActionResult> GetJobs()
        {
            return Ok(await _service.GetJobs());
        }

        [HttpPost("jobs")]
        public async Task<IActionResult> CreateJob(JobPost job)
        {
            await _service.CreateJob(job);
            return Ok(new { message = "Job created" });
        }

        [HttpPut("jobs/{id}")]
        public async Task<IActionResult> UpdateJob(Guid id, JobPost job)
        {
            await _service.UpdateJob(id, job);
            return Ok(new { message = "Job updated" });
        }

        [HttpDelete("jobs/{id}")]
        public async Task<IActionResult> DeleteJob(Guid id)
        {
            await _service.DeleteJob(id);
            return Ok(new { message = "Job deleted" });
        }

        // Applicants
        [HttpGet("jobs/{jobId}/applicants")]
        public async Task<IActionResult> GetApplicants(Guid jobId)
        {
            return Ok(await _service.GetApplicants(jobId));
        }

        [HttpPost("jobs/{jobId}/apply")]
        public async Task<IActionResult> Apply(Guid jobId, Applicant applicant)
        {
            await _service.Apply(jobId, applicant);
            return Ok(new { message = "Application submitted" });
        }

        [HttpPut("applicants/{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, ApplicationStatus status)
        {
            await _service.UpdateStatus(id, status);
            return Ok(new { message = "Status updated" });
        }
    }
}
