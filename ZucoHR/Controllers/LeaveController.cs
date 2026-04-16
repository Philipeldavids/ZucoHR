using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Shared;

namespace ZucoHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;
        private readonly ILogger<LeaveController> _logger;
        private readonly ITenantService _tenantService;

        public LeaveController(ILeaveService leaveService, ILogger<LeaveController> logger, ITenantService tenantService)
        {
            _leaveService = leaveService;
            _logger = logger;
            _tenantService = tenantService;
        }

        /// <summary>
        /// Request a new leave
        /// </summary>
        [HttpPost("request")]
        public async Task<IActionResult> RequestLeave([FromBody] LeaveRequestDto dto)
        {
            var validator = new LeaveRequestDtoValidator();
            var result = await validator.ValidateAsync(dto);

            if (!result.IsValid)
                return BadRequest(result.Errors.Select(e => e.ErrorMessage));

            var employeeId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException());

            var leave = await _leaveService.RequestLeaveAsync(employeeId, dto.StartDate, dto.EndDate, dto.Reason);
            return CreatedAtAction(nameof(GetById), new { id = leave.Id }, leave);
        }

        /// <summary>
        /// Get leave request by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // You can later connect this to a service-level fetch with role-based filtering.
            return Ok(new { message = "Leave fetched successfully.", id });
        }

        /// <summary>
        /// Approve a leave request
        /// </summary>
        [HttpPost("{leaveId:guid}/approve")]
        [Authorize(Roles = "Manager,HR")]
        public async Task<IActionResult> ApproveLeave(Guid leaveId)
        {
            var approverId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException());
            await _leaveService.ApproveAsync(leaveId, approverId);
            return Ok(new { message = "Leave approved successfully." });
        }

        /// <summary>
        /// Reject a leave request
        /// </summary>
        [HttpPost("{leaveId:guid}/reject")]
        [Authorize(Roles = "Manager,HR")]
        public async Task<IActionResult> RejectLeave(Guid leaveId, [FromBody] RejectLeaveDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Comment))
                return BadRequest("Comment is required for rejection.");

            var approverId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException());
            await _leaveService.RejectAsync(leaveId, approverId, dto.Comment);
            return Ok(new { message = "Leave rejected successfully." });
        }
    }
}
