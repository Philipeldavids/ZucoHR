using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Infrastructure
{
    public static class SeedData
    {
        public static async Task SeedRolesForOrganization(
    ZucoHrDbContext context,
    Guid organizationId)
        {
            if (context.Roles.Any(r => r.OrganizationId == organizationId))
                return;

            var roles = new List<Role>
    {
        new() { Id = Guid.NewGuid().ToString(), Name = "Admin", OrganizationId = organizationId },
        new() {  Id = Guid.NewGuid().ToString(), Name = "HR", OrganizationId = organizationId },
        new() {  Id = Guid.NewGuid().ToString(), Name = "Manager", OrganizationId = organizationId },
        new() { Id = Guid.NewGuid().ToString(), Name = "Employee", OrganizationId = organizationId }
    };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();

            await AssignDefaultPermissions(context, roles);
        }
        private static async Task AssignDefaultPermissions(
     ZucoHrDbContext context,
     List<Role> roles)
        {
            var permissions = await context.Permissions.ToListAsync();

            var rolePermissions = new List<RolePermission>();

            foreach (var role in roles)
            {
                var roleName = role.Name.ToUpper();

                foreach (var perm in permissions)
                {
                    var code = perm.Code.ToUpper();

                    bool shouldAssign = roleName switch
                    {
                        "ADMIN" => true,

                        "HR" => code.StartsWith("EMPLOYEE")
                                || code.StartsWith("PAYROLL")
                                || code.Contains("MANAGE"),

                        "MANAGER" => code.Contains("APPROVE"),

                        "EMPLOYEE" => code.Contains("VIEW"),

                        _ => false
                    };

                    if (!shouldAssign)
                        continue;

                    // ✅ Prevent duplicates (important)
                    bool exists = await context.RolePermissions.AnyAsync(rp =>
                        rp.RoleId == role.Id &&
                        rp.PermissionId == perm.Id);

                    if (!exists)
                    {
                        rolePermissions.Add(new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = perm.Id,
                            Permission = perm
                        });
                    }
                }
            }

            if (rolePermissions.Any())
            {
                await context.RolePermissions.AddRangeAsync(rolePermissions);
                await context.SaveChangesAsync();
            }
        }
        public static async Task Initialize(ZucoHrDbContext context)
        {
            if (!context.Permissions.Any())
            {
                await SeedPermissions(context);
                await SeedFeatures(context);
                await SeedPlans(context);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedPermissions(ZucoHrDbContext context)
        {
            var permissions = new List<Permission>
    {
        new() { Id = Guid.NewGuid().ToString(), Code = "EMPLOYEE_VIEW" },
        new() { Id = Guid.NewGuid().ToString(), Code = "EMPLOYEE_CREATE" },

        new() { Id = Guid.NewGuid().ToString(), Code = "PAYROLL_VIEW" },
        new() { Id = Guid.NewGuid().ToString(), Code = "PAYROLL_RUN" },

        new() { Id = Guid.NewGuid().ToString(), Code = "LEAVE_APPROVE" },

        new() { Id = Guid.NewGuid().ToString(), Code = "EXPENSE_APPROVE" },

        new() { Id = Guid.NewGuid().ToString(), Code = "RECRUITMENT_MANAGE" },
        new() { Id = Guid.NewGuid().ToString(), Code = "PERFORMANCE_MANAGE" },
        new() { Id = Guid.NewGuid().ToString(), Code = "ONBOARDING_MANAGE" }
    };

            await context.Permissions.AddRangeAsync(permissions);
        }

        private static async Task SeedFeatures(ZucoHrDbContext context)
        {
            var features = new List<Feature>
    {
        new() { Id = Guid.NewGuid(), Code = "EMPLOYEE" },
        new() { Id = Guid.NewGuid(), Code = "PAYROLL" },
        new() { Id = Guid.NewGuid(), Code = "LEAVE" },
        new() { Id = Guid.NewGuid(), Code = "EXPENSE" },
        new() { Id = Guid.NewGuid(), Code = "RECRUITMENT" },
        new() { Id = Guid.NewGuid(), Code = "PERFORMANCE" },
        new() { Id = Guid.NewGuid(), Code = "ONBOARDING" }
    };

            await context.Features.AddRangeAsync(features);
        }

        private static async Task SeedPlans(ZucoHrDbContext context)
        {
            var features = await context.Features.ToListAsync();

            var starter = new SubscriptionPlan
            {
                
                Name = "Starter",
                Price = 0
            };

            var growth = new SubscriptionPlan
            {
                
                Name = "Growth",
                Price = 50000
            };

            var enterprise = new SubscriptionPlan
            {
                
                Name = "Enterprise",
                Price = 150000
            };

            await context.SubscriptionPlans.AddRangeAsync(starter, growth, enterprise);
            await context.SaveChangesAsync();

            // Map features
            var planFeatures = new List<PlanFeature>();

            foreach (var feature in features)
            {
                if (feature.Code == "EMPLOYEE" || feature.Code == "LEAVE" || feature.Code == "PAYROLL")
                {
                    planFeatures.Add(new PlanFeature
                    {
                        PlanId = starter.Id,
                        FeatureId = feature.Id
                    });
                }

                if (feature.Code != "RECRUITMENT" || feature.Code!= "ONBOARDING")
                {
                    planFeatures.Add(new PlanFeature
                    {
                        PlanId = growth.Id,
                        FeatureId = feature.Id
                    });
                }

                // Enterprise → all features
                planFeatures.Add(new PlanFeature
                {
                    PlanId = enterprise.Id,
                    FeatureId = feature.Id
                });
            }

            await context.PlanFeatures.AddRangeAsync(planFeatures);
        }
    }


}