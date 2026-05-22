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

        public DbSet<PlanFeature> PlanFeatures { get; set; }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Role> Roles {  get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<OrganizationSubscription> OrgSubscription { get; set; }
        public DbSet<UserRole> UserRoles {  get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }
        public DbSet<ReviewCompetency> ReviewCompetencies { get; set; }
        public DbSet<ReviewGoal> ReviewGoals { get; set; }
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<JobPost> JobPosts => Set<JobPost>();
        public DbSet<Applicant> Applicants => Set<Applicant>();
       // public DbSet<Onboarding> Onboardings { get; set; }
        public DbSet<OnboardingTask> OnboardingTasks { get; set; }
        public DbSet<OnboardingDocument> OnboardingDocuments { get; set; }
        public DbSet<PayRun> PayRuns => Set<PayRun>();
        public DbSet<Payslip> Payslips => Set<Payslip>();
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlanFeature>().HasKey(p => p.PlanId);
            modelBuilder.Entity<Organization>().HasKey(o => o.Id);
            //modelBuilder.Entity<Role>().HasKey(o => o.Id);
            modelBuilder.Entity<UserRole>().HasKey(u => u.UserId);
            modelBuilder.Entity<Feature>().HasKey(f=>f.Id);
            modelBuilder.Entity<RolePermission>()
    .HasKey(rp => new { rp.RoleId, rp.PermissionId });
            modelBuilder.Entity<OrganizationSubscription>().HasKey(o=>o.Id);
            modelBuilder.Entity<SubscriptionPlan>().HasKey(f=>f.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(e => e.EmployeeNumber).IsUnique();
            modelBuilder.Entity<LeaveRequest>().HasKey(e => e.Id);

            modelBuilder.Entity<ReviewCompetency>()
    .HasOne(x => x.PerformanceReview)
    .WithMany(x => x.Competencies)
    .HasForeignKey(x => x.PerformanceReviewId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReviewGoal>()
                .HasOne(x => x.PerformanceReview)
                .WithMany(x => x.Goals)
                .HasForeignKey(x => x.PerformanceReviewId)
                .OnDelete(DeleteBehavior.Cascade); 
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
                    .HasConversion<string>();
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

                entity.Property(x => x.Stage)
                    .HasConversion<string>();

                entity.HasOne<JobPost>()
                    .WithMany()
                    .HasForeignKey(x => x.JobPostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            

            modelBuilder.Entity<OnboardingTask>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                //entity.HasOne<Employee>();
                    
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
