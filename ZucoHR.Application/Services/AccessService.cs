using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZucoHR.Application.Interfaces;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Application.Services
{

    public class AccessService : IAccessService
    {
        private readonly ZucoHrDbContext _context;
        private readonly ITenantService _tenantService;

        public AccessService(
            ZucoHrDbContext context,
            ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        // 🔐 MAIN METHOD (USE THIS EVERYWHERE)
        public async Task<bool> HasPermission(string userId, string permissionCode)
        {
            var organizationId = _tenantService.GetTenantId();

            // 1️⃣ Check if permission exists
            var permission = await _context.Permissions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == permissionCode);

            if (permission == null)
                return false;

            // 2️⃣ Check user role permissions
            var hasRolePermission = await (
                from ur in _context.UserRoles
                join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
                join r in _context.Roles on ur.RoleId equals r.Id
                where ur.UserId == userId
                    && rp.PermissionId == permission.Id
                    && r.OrganizationId == organizationId
                select rp
            ).AnyAsync();

            if (!hasRolePermission)
                return false;

            // 3️⃣ Check subscription feature
            var featureCode = ExtractFeatureFromPermission(permissionCode);

            var hasFeature = await HasFeature(organizationId, featureCode);

            return hasFeature;
        }

        // 🔓 Feature Check (Plan-based)
        public async Task<bool> HasFeature(Guid organizationId, string featureCode)
        {
            var now = DateTime.UtcNow;

            var hasFeature = await (
                from sub in _context.OrgSubscription
                join pf in _context.PlanFeatures on sub.PlanId equals pf.PlanId
                join f in _context.Features on pf.FeatureId equals f.Id
                where sub.OrganizationId == organizationId
                    && sub.IsActive
                    && sub.StartDate <= now
                    && sub.EndDate >= now
                    && f.Code == featureCode
                select f
            ).AnyAsync();

            return hasFeature;
        }

        // 🧠 Map Permission → Feature
        private string ExtractFeatureFromPermission(string permissionCode)
        {
            // Convention: PAYROLL_RUN → PAYROLL
            return permissionCode.Split('_')[0];
        }
    }
}