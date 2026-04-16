using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;

namespace ZucoHR.Infrastructure.Repository
{
        public class UserRepository : IUserRepository
        {
            private readonly ZucoHrDbContext _ctx;
            public UserRepository(ZucoHrDbContext ctx) { _ctx = ctx; }

            public async Task<User?> GetByEmailAsync(string email)
            {
                return await _ctx.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email);
            }
            public async Task<User?> GetByIdAsync(string id) => await _ctx.Users.FindAsync(id);
            public async Task<User> AddAsync(User user) { _ctx.Users.Add(user); await _ctx.SaveChangesAsync(); return user; }
            public async Task UpdateAsync(User user) { _ctx.Users.Update(user); await _ctx.SaveChangesAsync(); }
        }
    }

