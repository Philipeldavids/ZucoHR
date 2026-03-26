using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Infrastructure.Data
{
    public class ZucoHrDbContext : IdentityDbContext<User>
    {
        public ZucoHrDbContext(DbContextOptions<ZucoHrDbContext> options) : base(options)
        { }

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
        public DbSet<PerformanceReview> PerformanceReviews => Set<PerformanceReview>();
        public DbSet<ExpenseClaim> ExpenseClaims => Set<ExpenseClaim>();
        public DbSet<JobRequisition> JobRequisitions => Set<JobRequisition>();
        public DbSet<Application> Applications => Set<Application>();
        public DbSet<OnboardingChecklistItem> OnboardingItems => Set<OnboardingChecklistItem>();
        public DbSet<PayRun> PayRuns => Set<PayRun>();
        public DbSet<Payslip> Payslips => Set<Payslip>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(e => e.EmployeeNumber).IsUnique();

            modelBuilder.Entity<ExpenseClaim>().Property(e => e.Amount).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PayRun>()
            .HasMany(p => p.Payslips)
            .WithOne(s => s.PayRun)
            .HasForeignKey(s => s.PayRunId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payslip>().Property(p => p.NetPay).HasColumnType("decimal(18,2)");
        }
    }
}
