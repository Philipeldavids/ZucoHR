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
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<ZucoHrDbContext>();

            var roles = new[] { "Admin", "HR", "Manager", "Employee", "Finance" };
            foreach (var r in roles)
            {
                if (!ctx.Roles.Any(x => x.Name == r)) ctx.Roles.Add(new Role { Id = Guid.NewGuid(), Name = r });
            }

            if (!ctx.Users.Any(u => u.Email == "admin@zucohr.local"))
            {
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@zucohr.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("ChangeMe123!"),
                    Role = "Admin"
                };
                ctx.Users.Add(admin);
            }

            if (!ctx.Employees.Any())
            {
                ctx.Employees.Add(new Employee
                {
                    Id = Guid.NewGuid(),
                    EmployeeNumber = "ZU-0001",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@zucohr.local",
                    HireDate = DateTime.UtcNow.AddYears(-1),
                    BaseSalary = 250000
                });
            }

            await ctx.SaveChangesAsync();
        }
    }

}
