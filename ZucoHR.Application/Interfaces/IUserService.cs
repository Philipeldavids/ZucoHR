using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> CreateRole(CreateRoleRequest request);
        Task<bool> AddUser(CreateUserRequest request);
        Task<List<Permission>> GetPermissions();
        Task<PaginatedResponse<Role>> GetRoles(int page = 1, int pageSize = 10);
        Task<PaginatedResponse<UserDto>> GetUsers(int page = 1, int pageSize = 10);
        Task AssignRole(string userId, string roleId);
        Task RemoveRole(string userId, string roleId);
        Task ToggleUserStatus(string userId, bool isActive);
    }
}
