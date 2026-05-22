using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Services;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
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
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RequestLeave([FromBody] LeaveRequestDto dto)
        {
            try
            {
                var validator = new LeaveRequestDtoValidator();
                var result = await validator.ValidateAsync(dto);

                if (!result.IsValid)
                    return BadRequest(result.Errors.Select(e => e.ErrorMessage));

                var employeeId = Guid.Parse(User.FindFirst("employeeId")?.Value ?? throw new UnauthorizedAccessException());
                var leave = await _leaveService.RequestLeaveAsync(employeeId, dto.Type, dto.StartDate, dto.EndDate, dto.Reason);
                return CreatedAtAction(nameof(GetById), new { id = leave.Id }, leave);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            try
            {
                var orgId = _tenantService.GetTenantId();
                var leaves = await _leaveService.GetAll(orgId, page, pageSize);
                return Ok(leaves);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin,Manager,HR, HR Manager")]
        public async Task<IActionResult> ApproveLeave(string id)
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
                 ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new UnauthorizedAccessException("Approval ID claim missing"); 
            var approverId = Guid.Parse(claim.Value);
            await _leaveService.ApproveAsync(Guid.Parse(id), approverId);
            return Ok(new { message = "Leave approved successfully." });
            
        }

        /// <summary>
        /// Reject a leave request
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin,Manager,HR, HR Manager")]
        public async Task<IActionResult> RejectLeave(string id, string comment = null)
        {
            //if (string.IsNullOrWhiteSpace(dto.Comment))
            //    return BadRequest("Comment is required for rejection.");

            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new UnauthorizedAccessException("Approval ID claim missing");
            var approverId = Guid.Parse(claim.Value);
            await _leaveService.RejectAsync(Guid.Parse(id), approverId, comment);
            return Ok(new { message = "Leave rejected successfully." });
        }
    }
}
