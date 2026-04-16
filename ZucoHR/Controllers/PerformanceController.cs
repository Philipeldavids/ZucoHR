using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceService _service;

        public PerformanceController(IPerformanceService service)
        {
            _service = service;
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeReviews(Guid employeeId)
        {
            var result = await _service.GetEmployeeReviews(employeeId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(Guid id)
        {
            var result = await _service.GetReview(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PerformanceReview review)
        {
            await _service.CreateReview(review);
            return Ok(new { message = "Created successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PerformanceReview review)
        {
            await _service.UpdateReview(id, review);
            return Ok(new { message = "Updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteReview(id);
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
