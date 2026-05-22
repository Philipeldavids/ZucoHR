using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
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

        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var res = await _service.GetReviews(page, pageSize);
                return Ok(res);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> Create(PerformanceReviewRequest review)
        {
            try
            {
                var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
                 ?? User.FindFirst(ClaimTypes.NameIdentifier);

                if (claim == null)
                    throw new UnauthorizedAccessException("User ID claim missing");
                var userId = Guid.Parse(claim.Value);
                review.ReviewerId = userId;
                await _service.CreateReview(review);
                return Ok(new { message = "Created successfully" });

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> Update(Guid id, PerformanceReviewRequest review)
        {
            try
            {
                var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
                 ?? User.FindFirst(ClaimTypes.NameIdentifier);

                if (claim == null)
                    throw new UnauthorizedAccessException("User ID claim missing");
                var userId = Guid.Parse(claim.Value);
                review.ReviewerId = userId;
                await _service.UpdateReview(id, review);
                return Ok(new { message = "Updated successfully" });

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
             }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteReview(id);
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
