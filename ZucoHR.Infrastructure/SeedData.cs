using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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


   


    public static class SuperAdminSeeder
    {
        public static async Task SeedAsync(
            ZucoHrDbContext context,
            IConfiguration configuration)
        {
            var email = configuration["BootstrapSuperAdmin:Email"];
            var password = configuration["BootstrapSuperAdmin:Password"];
            var name = configuration["BootstrapSuperAdmin:Name"]
                       ?? "Platform Administrator";

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Bootstrap Super Admin credentials are not configured.");
            }

            if (password.Length < 12)
            {
                throw new InvalidOperationException(
                    "Bootstrap Super Admin password must be at least 12 characters.");
            }

            email = email.Trim();

            // Find or create the platform organization.
            var organization = await context.Organizations
                .FirstOrDefaultAsync(o => o.Slug == "zucohr-platform");

            if (organization == null)
            {
                organization = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = "ZucoHR Platform",
                    Slug = "zucohr-platform",
                    CurrencyCode = "NGN",
                    CurrencySymbol = "₦",
                    Timezone = "Africa/Lagos",
                    CreatedAt = DateTime.UtcNow
                };

                context.Organizations.Add(organization);
                await context.SaveChangesAsync();
            }

            // Check whether this account already exists.
            var existingUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                if (!string.Equals(
                    existingUser.Role,
                    "SuperAdmin",
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "The configured email already belongs to another user. " +
                        "No automatic role promotion was performed.");
                }

                // Do not reset an existing administrator's password on startup.
                return;
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = email,
                NormalizedUserName = email.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true,

                Name = name,
                OrganizationId = organization.Id,

                Role = "SuperAdmin",
                IsActive = true,

                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),

                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
    public static class SeedData
    {
        public static async Task SeedRolesForOrganization(
    ZucoHrDbContext context,
    Guid organizationId)
        {
            var defaultRoles = new[]
            {
        "Admin",
        "HR",
        "Manager",
        "Employee"
    };

            var existingRoles = await context.Roles
                .Where(r => r.OrganizationId == organizationId)
                .Select(r => r.Name)
                .ToListAsync();

            var rolesToCreate = defaultRoles
                .Where(name => !existingRoles.Contains(
                    name,
                    StringComparer.OrdinalIgnoreCase))
                .Select(name => new Role
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    NormalizedName = name.ToUpperInvariant(),
                    OrganizationId = organizationId
                })
                .ToList();

            if (!rolesToCreate.Any())
                return;

            await context.Roles.AddRangeAsync(rolesToCreate);
            await context.SaveChangesAsync();

            await AssignDefaultPermissions(context, rolesToCreate);
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

            var freePlan = new SubscriptionPlan
            {
                Name = "1 month Free Trial",
                Price = 0
            };
            var starter = new SubscriptionPlan
            {
                
                Name = "Starter",
                Price = 350000
            };

            var growth = new SubscriptionPlan
            {
                
                Name = "Growth",
                Price = 500000
            };

            var enterprise = new SubscriptionPlan
            {
                
                Name = "Enterprise",
                Price = 50000000
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