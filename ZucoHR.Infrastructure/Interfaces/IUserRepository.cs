using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<PaginatedResponse<User>> GetAllUsers(int page, int pageSize);
        Task<bool> CreateRole(Role role);
        Task<List<Permission>> GetPermission();
        Task<List<Role>> GetRoles(Guid OrgId);
        Task UpdateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id, Guid OrgId);
        Task<User> AddAsync(User user);
        Task<List<User>> GetAllByOrganizationAsync(Guid orgId);
    }
}
