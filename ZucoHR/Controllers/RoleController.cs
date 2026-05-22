using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Utilities;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Controllers
{
    [RequirePermission("EMPLOYEE_CREATE")]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ZucoHrDbContext _context;
        private readonly ITenantService _tenantService;

        public RoleController(ZucoHrDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        // ✅ Get all roles in organization
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var orgId = _tenantService.GetTenantId();

            var roles = await _context.Roles
                .Where(r => r.OrganizationId == orgId)
                .ToListAsync();

            return Ok(roles);
        }

        // ✅ Create new role
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleRequest request)
        {
            var orgId = _tenantService.GetTenantId();

            var role = new Role
            {
                
                Name = request.Name,
                OrganizationId = orgId
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return Ok(role);
        }

        // ✅ Assign role to user
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole(AssignRoleRequest request)
        {
            var exists = await _context.UserRoles
                .AnyAsync(x => x.UserId == request.UserId && x.RoleId == request.RoleId);

            if (exists)
                return BadRequest("User already has this role");

            var userRole = new UserRole
            {
                UserId = request.UserId,
                RoleId = request.RoleId
            };

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Role assigned" });
        }

        // ✅ Remove role from user
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveRole(string userId, string roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);

            if (userRole == null)
                return NotFound();

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Role removed" });
        }

        // ✅ Update role permissions
        [HttpPost("permissions")]
        public async Task<IActionResult> UpdatePermissions(UpdateRolePermissionsRequest request)
        {
            var existing = _context.RolePermissions
                .Where(x => x.RoleId == request.RoleId);

            _context.RolePermissions.RemoveRange(existing);

            var newPermissions = request.PermissionIds.Select(pid => new RolePermission
            {
                RoleId = request.RoleId,
                PermissionId = pid
            });

            await _context.RolePermissions.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Permissions updated" });
        }
    }
}
