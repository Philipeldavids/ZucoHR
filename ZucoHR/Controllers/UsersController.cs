using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Services;
using ZucoHR.Application.Utilities;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ITenantService _tenantService;
        private readonly ZucoHrDbContext _context;

        public UsersController(ZucoHrDbContext context,IUserService service, ITenantService tenantService)
        {
            _service = service;
            _tenantService = tenantService;
            _context = context;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(int page, int pageSize) {
            var users = await _service.GetAllUsers(page, pageSize);

            return Ok(users);
        
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
[FromBody] AdminResetPasswordDto dto)
        {
            var user =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Id == dto.UserId
                    );

            if (user == null)
                return NotFound();

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.NewPassword
                );

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("create")]
        [Authorize(Roles =("Admin, HR, HR Manager"))]

        public async Task<IActionResult> CreateRole(CreateRoleRequest req)
        {
            try
            {
                var res = await _service.CreateRole(req);
                    return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("permissions")]
        [Authorize(Roles = ("Admin, HR, HR Manager"))]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                var permissions = await _service.GetPermissions();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetRoles")]
        [Authorize(Roles = ("Admin, HR, HR Manager"))]
        public async Task<IActionResult> GetRoles()
        {

            try
            {
                var roles = await _service.GetRoles();
                return Ok(roles);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Authorize(Roles = ("Admin, HR, HR Manager"))]
        public async Task<IActionResult> Adduser(CreateUserRequest request)
        {
            var OrgClaim = User.FindFirst("OrganizationId").Value;
            if (string.IsNullOrWhiteSpace(OrgClaim))
                throw new UnauthorizedAccessException("Organization claim missing");
            var orgIdFromToken = Guid.Parse(OrgClaim);

            if (orgIdFromToken != _tenantService.GetTenantId())
                throw new Exception("Unauthorized");

            var res= _service.AddUser(request);

            return Ok(res);
        }
        // ✅ Get all users in organization
        [HttpGet]
        [Authorize(Roles = ("Admin, HR, HR Manager"))]
        public async Task<IActionResult> GetUsers([FromQuery] int page= 1, [FromQuery] int pageSize= 10)
        {
            var OrgClaim = User.FindFirst("OrganizationId")?.Value;
            if (string.IsNullOrWhiteSpace(OrgClaim))
                throw new UnauthorizedAccessException("Organization claim missing");
            var orgIdFromToken = Guid.Parse(OrgClaim);

            if (orgIdFromToken != _tenantService.GetTenantId())
                throw new Exception("Unauthorized");
            var users = await _service.GetUsers(page, pageSize);
            return Ok(users);
        }
        [HttpDelete("{Id}")]
        [Authorize(Roles = ("Admin, HR, HR Manager"))]

        public async Task<IActionResult> Delete(string Id)
        {
            await _service.DeleteAsync(Id);
            return Ok();
        }
        // ✅ Assign role
        [HttpPost("assign-role")]
        [Authorize(Roles = ("Admin, HR, HR Manager"))]
        public async Task<IActionResult> AssignRole(AssignUserRoleRequest request)
        {
            var OrgClaim = User.FindFirst("OrganizationId")?.Value;
            if (string.IsNullOrWhiteSpace(OrgClaim))
                throw new UnauthorizedAccessException("Organization claim missing");
            var orgIdFromToken = Guid.Parse(OrgClaim);

            if (orgIdFromToken != _tenantService.GetTenantId())
                throw new Exception("Unauthorized");
            await _service.AssignRole(request.UserId, request.RoleId);
            return Ok(new { message = "Role assigned" });
        }

        // ✅ Remove role
        [HttpPost("remove-role")]
        [Authorize(Roles = ("Admin, HR, HR Manager"))]
        public async Task<IActionResult> RemoveRole(AssignUserRoleRequest request)
        {
            var OrgClaim = User.FindFirst("OrganizationId").Value;
            if (string.IsNullOrWhiteSpace(OrgClaim))
                throw new UnauthorizedAccessException("Organization claim missing");
            var orgIdFromToken = Guid.Parse(OrgClaim);

            if (orgIdFromToken != _tenantService.GetTenantId())
                throw new Exception("Unauthorized");
            await _service.RemoveRole(request.UserId, request.RoleId);
            return Ok(new { message = "Role removed" });
        }

        // ✅ Activate / Deactivate user
        [HttpPost("toggle-status")]
        [Authorize(Roles = ("Admin, HR, HR Manager"))]
        public async Task<IActionResult> ToggleStatus(ToggleUserStatusRequest request)
        {
            var OrgClaim = User.FindFirst("OrganizationId").Value;
            if (string.IsNullOrWhiteSpace(OrgClaim))
                throw new UnauthorizedAccessException("Organization claim missing");
            var orgIdFromToken = Guid.Parse(OrgClaim);
            
            if (orgIdFromToken != _tenantService.GetTenantId())
                throw new Exception("Unauthorized");
            await _service.ToggleUserStatus(request.UserId, request.IsActive);
            return Ok(new { message = "User status updated" });
        }
    }
}
