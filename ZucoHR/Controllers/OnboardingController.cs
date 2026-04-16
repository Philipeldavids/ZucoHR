using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService _service;

        public OnboardingController(IOnboardingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start(Guid applicantId)
        {
            await _service.StartOnboarding(applicantId);
            return Ok(new { message = "Onboarding started" });
        }

        [HttpPost("{id}/tasks")]
        public async Task<IActionResult> AddTask(Guid id, OnboardingTask task)
        {
            await _service.AddTask(id, task);
            return Ok(new { message = "Task added" });
        }

        [HttpPost("tasks/{taskId}/complete")]
        public async Task<IActionResult> CompleteTask(Guid taskId)
        {
            await _service.CompleteTask(taskId);
            return Ok(new { message = "Task completed" });
        }

        [HttpPost("{id}/documents")]
        public async Task<IActionResult> UploadDoc(Guid id, OnboardingDocument doc)
        {
            await _service.UploadDocument(id, doc);
            return Ok(new { message = "Document uploaded" });
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> Complete(Guid id)
        {
            await _service.CompleteOnboarding(id);
            return Ok(new { message = "Onboarding completed" });
        }
    }
}
