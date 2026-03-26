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
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ZucoHrDbContext _ctx;
        public RefreshTokenRepository(ZucoHrDbContext ctx) { _ctx = ctx; }

        public async Task AddAsync(RefreshToken token) { _ctx.RefreshTokens.Add(token); await _ctx.SaveChangesAsync(); }
        public async Task<RefreshToken?> GetByTokenAsync(string token) => await _ctx.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token);
        public async Task RevokeAsync(RefreshToken token) { token.IsRevoked = true; _ctx.RefreshTokens.Update(token); await _ctx.SaveChangesAsync(); }
        public async Task RemoveExpiredAsync()
        {
            var expired = _ctx.RefreshTokens.Where(t => t.ExpiresAt <= DateTime.UtcNow);
            _ctx.RefreshTokens.RemoveRange(expired);
            await _ctx.SaveChangesAsync();
        }
    }
}
