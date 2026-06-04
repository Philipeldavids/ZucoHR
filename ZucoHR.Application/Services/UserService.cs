using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;

namespace ZucoHR.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ZucoHrDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly INotificationService _notificationService;

        public UserService(
            IUserRepository repo,
            INotificationService notificationService,
            ZucoHrDbContext context,
            ITenantService tenantService)
        {
            _repo = repo;
            _context = context;
            _notificationService = notificationService;
            _tenantService = tenantService;
        }

        public async Task<bool> CreateRole(CreateRoleRequest request)
        {
            var orgId = _tenantService.GetTenantId();
            var role = new Role();

            role.Name = request.Name;
            role.OrganizationId = orgId;

            foreach (var item in role.RolePermissions)
            {
                foreach(var req in request.PermissionIds)
                {
                    item.RoleId = Guid.NewGuid().ToString();
                    item.PermissionId = req.ToString();
                    var permission = await _context.Permissions.Where(x => x.Id == item.PermissionId).FirstOrDefaultAsync();
                    item.Permission = permission;
                }
               
            }
            var res = await _repo.CreateRole(role);
            return res;
        }
        public async Task<List<Permission>> GetPermissions()
        {
            var permissions = await _repo.GetPermission();

            return permissions;
        }
            public async Task<PaginatedResponse<Role>> GetRoles(int page= 1, int pageSize = 10)
        {
            var orgId = _tenantService.GetTenantId();
            var roles = await _repo.GetRoles(orgId);
            return new PaginatedResponse<Role>
            {
                Data = roles,
                Total = roles.Count,
                Page = page,
                PageSize = pageSize

            };
        }
        public async Task<bool> AddUser(CreateUserRequest request)
        {
            var orgId = _tenantService.GetTenantId();

            User user = new User();
            user.Email = request.Email;
            user.Name = request.Name;
            user.UserName = request.Email;
            user.OrganizationId = orgId;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _repo.AddAsync(user);

            return true;
        }
        public async Task<PaginatedResponse<UserDto>> GetUsers(int page=1, int pageSize = 10)
        {
            
            var orgId = _tenantService.GetTenantId();
            //var userrole = new UserRole();

            var query = _context.Users.AsQueryable()
                .Where(u => u.OrganizationId == orgId)
                .Include(r => r.RefreshTokens);


                 var items = await query                
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();



            List<UserDto> userDto = new List<UserDto>();
            foreach (var user in items)
            {
                userDto.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Role = user.Role
                });                   
                
              }
            return new PaginatedResponse<UserDto>
            {
                Data = userDto,
                Total = userDto.Count,
                Page = page,
                PageSize = pageSize
                //userrole = await _context.UserRoles.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
            };


        }
        public async Task<PaginatedResponse<User>> GetAllUsers(int page, int pageSize)
        {
            var users = await _repo.GetAllUsers(page, pageSize);
            return users;
        }
        public async Task AssignRole(string userId, string roleId)
        {
            var exists = await _context.UserRoles
                .AnyAsync(x => x.UserId == userId && x.RoleId == roleId);

            if (exists)
                throw new Exception("User already has role");

            var user = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

           var role = await _context.Roles.Where(x => x.Id == roleId).FirstOrDefaultAsync();
            if(user != null && role != null)
            {
                user.Role = role.Name;
            }
            

             _context.Users.Update(user);
            await _context.SaveChangesAsync();
            await _notificationService.CreateAsync(
    userId.ToString(),
    "Role Assigned",
    $"You have been assigned the role {role.Name}."
);


            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                Role = role
                
            };

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRole(string userId, string role)
        {
            var roleId = await _context.Roles.Where(x => x.Name == role).Select(x => x.Id).FirstOrDefaultAsync();
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);

            if (userRole == null)
                throw new Exception("Role not found");
             _context.UserRoles.Remove(userRole);
             await _context.SaveChangesAsync();

            var user = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            user.Role = "";
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task ToggleUserStatus(string userId, bool isActive)
        {
           var orgId = _tenantService.GetTenantId();
            var user = await _repo.GetByIdAsync(userId, orgId);

            if (user == null)
                throw new Exception("User not found");

            user.IsActive = isActive;

            await _repo.UpdateAsync(user);
        }

        public async Task DeleteAsync(string userId)
        {
            var orgId = _tenantService.GetTenantId();
            var user = await _repo.GetByIdAsync(userId, orgId);

            if (user == null) throw new Exception("User does not exist");

            _repo.Delete(user);
        }
    }
}
