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
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<JobPost> JobPosts => Set<JobPost>();
        public DbSet<Applicant> Applicants => Set<Applicant>();
        public DbSet<Onboarding> Onboardings { get; set; }
        public DbSet<OnboardingTask> OnboardingTasks { get; set; }
        public DbSet<OnboardingDocument> OnboardingDocuments { get; set; }
        public DbSet<PayRun> PayRuns => Set<PayRun>();
        public DbSet<Payslip> Payslips => Set<Payslip>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(e => e.EmployeeNumber).IsUnique();

            
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.Category)
                    .HasMaxLength(100);

                entity.Property(x => x.Status)
                    .HasConversion<int>();
            });

            modelBuilder.Entity<PayRun>()
            .HasMany(p => p.Payslips)
            .WithOne(s => s.PayRun)
            .HasForeignKey(s => s.PayRunId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payslip>().Property(p => p.NetPay).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<PerformanceReview>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.ReviewPeriod).IsRequired().HasMaxLength(50);
                entity.Property(x => x.Score).IsRequired();
                entity.Property(x => x.Summary).HasMaxLength(1000);
            });
            modelBuilder.Entity<JobPost>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .IsRequired();

                entity.Property(x => x.Department)
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Applicant>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Email)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasConversion<int>();

                entity.HasOne<JobPost>()
                    .WithMany()
                    .HasForeignKey(x => x.JobPostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Onboarding>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<OnboardingTask>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne<Onboarding>()
                    .WithMany()
                    .HasForeignKey(x => x.OnboardingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OnboardingDocument>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.DocumentName)
                    .IsRequired()
                    .HasMaxLength(200);
            });
        }
    }
}
