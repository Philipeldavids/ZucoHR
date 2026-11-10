using Microsoft.AspNetCore.Identity;
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

namespace ZucoHR.Application.Services
{
    public class SuperAdminService : ISuperAdminService
    {
        private readonly ZucoHrDbContext _context;
        private readonly UserManager<User> _userManager;

        public SuperAdminService(
            ZucoHrDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<SuperAdminDashboardDto> GetDashboardAsync()
        {
            return new SuperAdminDashboardDto
            {
                TotalCompanies = await _context.Organizations.CountAsync(),

                TotalUsers = await _context.Users.CountAsync(),

                ActiveUsers = await _context.Users
                    .CountAsync(u => u.IsActive),

                InactiveUsers = await _context.Users
                    .CountAsync(u => !u.IsActive)
            };
        }

        public async Task<List<SuperAdminCompanyDto>> GetCompaniesAsync()
        {
            return await _context.Organizations
                .AsNoTracking()
                .Select(o => new SuperAdminCompanyDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Slug = o.Slug,
                    CurrencyCode = o.CurrencyCode,
                    CreatedAt = o.CreatedAt,

                    TotalUsers = _context.Users
                        .Count(u => u.OrganizationId == o.Id),

                    ActiveUsers = _context.Users
                        .Count(u =>
                            u.OrganizationId == o.Id &&
                            u.IsActive)
                })
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<List<SuperAdminUserDto>> GetUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Organization)
                .Select(u => new SuperAdminUserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email ?? "",
                    Role = u.Role,
                    IsActive = u.IsActive,
                    OrganizationId = u.OrganizationId,
                    OrganizationName = u.Organization != null
                        ? u.Organization.Name
                        : "Unknown organization"
                })
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<bool> ResetUserPasswordAsync(
    string userId,
    string newPassword)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            if (string.IsNullOrWhiteSpace(newPassword) ||
                newPassword.Length < 8)
            {
                throw new ArgumentException(
                    "Password must contain at least 8 characters.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Revoke existing refresh tokens so existing sessions
            // cannot continue using old credentials.
            var refreshTokens = await _context.RefreshTokens
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(refreshTokens);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            // Protect Super Admin accounts.
            if (string.Equals(
                user.Role,
                "SuperAdmin",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Super Admin accounts cannot be deleted from this screen.");
            }

            // Preserve historical HR and payroll records.
            user.IsActive = false;
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}