using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ZucoHR.Infrastructure.Repository
{
        public class UserRepository : IUserRepository
        {
            private readonly ZucoHrDbContext _ctx;
            public UserRepository(ZucoHrDbContext ctx) { _ctx = ctx; }

            public async Task<List<Role>> GetRoles(Guid OrgId)
        {
            return await _ctx.Roles.Where(x=>x.OrganizationId == OrgId).ToListAsync();
        }
            public async Task<User?> GetByEmailAsync(string email)
            {
                return await _ctx.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email);
            }
            public async Task<User?> GetByIdAsync(string id, Guid OrgId) => await _ctx.Users.Where(x=>x.Id == id && x.OrganizationId == OrgId)
            .Include(x=>x.RefreshTokens).FirstOrDefaultAsync();
            public async Task<User> AddAsync(User user) { _ctx.Users.Add(user); await _ctx.SaveChangesAsync(); return user; }
            public async Task UpdateAsync(User user) { _ctx.Users.Update(user); await _ctx.SaveChangesAsync(); }

        public async Task<List<User>> GetAllByOrganizationAsync(Guid orgId)
        {
            return await _ctx.Users
                .Where(u => u.OrganizationId == orgId)                
                .Include(r=> r.RefreshTokens)
                .ToListAsync();
        }

        public async Task<List<Permission>> GetPermission()
        {
            return await _ctx.Permissions.ToListAsync();
        }

        public async Task<bool> CreateRole(Role role)
        {
            _ctx.Roles.Add(role);
            await _ctx.SaveChangesAsync();

            return true;
        }

        public async Task Delete(User user)
        {
            _ctx.Users.Remove(user);
            await _ctx.SaveChangesAsync();
        }
        public async Task<PaginatedResponse<User>> GetAllUsers(int page, int pageSize)
        {
            var users =  _ctx.Users;

            var totalCount = await users.CountAsync();
            var items = await users
                .OrderBy(e => e.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<User>
            {
                Data = items,
                Total = totalCount,
                Page = page,
                PageSize = pageSize
            };
            
        }
    }
    }

