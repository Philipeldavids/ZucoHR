using Azure.Core;
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
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;

namespace ZucoHR.Application.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly IPerformanceRepository _repository;
        private readonly ITenantService _tenantService;
        private readonly IEmployeeService _service;
        private readonly ZucoHrDbContext _context;
        public PerformanceService(
            IPerformanceRepository repository, ZucoHrDbContext context,
            ITenantService tenantService, IEmployeeService service)
        {
            _repository = repository;
            _tenantService = tenantService;
            _service = service;
            _context = context;
        }

        public async Task<PaginatedResponse<PerformanceReview>> GetReviews(int page= 1, int pageSize =10) 
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetReviews(orgId, page, pageSize);
        }
        public async Task<List<PerformanceReview>> GetEmployeeReviews(Guid employeeId)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetByEmployeeAsync(orgId, employeeId);
        }

        public async Task<PerformanceReview?> GetReview(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetByIdAsync(orgId, id);
        }

        public async Task CreateReview(PerformanceReviewRequest reviewRequest)
        {
            var orgId = _tenantService.GetTenantId();
            var emp = await _service.GetByIdAsync(reviewRequest.EmployeeId, orgId);
            PerformanceReview review = new PerformanceReview();
            review.Id = Guid.NewGuid();
            review.OrganizationId = _tenantService.GetTenantId();
            review.ReviewPeriod = reviewRequest.ReviewPeriod;
            review.ReviewerId = reviewRequest.ReviewerId;
            review.Employee = emp;
            review.Summary = reviewRequest.Summary;
            review.Score = reviewRequest.Score;
            review.OrganizationId = orgId;
            review.ReviewerId = reviewRequest.ReviewerId;
            review.CreatedAt = DateTime.UtcNow;
            review.Status = reviewRequest.Status;
            review.Competencies = reviewRequest.Competencies.Select(x =>
    new ReviewCompetency
    {
        Id = Guid.NewGuid(),
        Label = x.Label,
        Score = x.Score
    }).ToList();

            review.Goals = reviewRequest.Goals.Select(x =>
                new ReviewGoal
                {
                    Id = Guid.NewGuid(),
                    Title = x.Title,
                    IsCompleted = x.IsCompleted
                }).ToList();
            await _repository.CreateAsync(review);
        }


        public async Task UpdateReview(Guid id, PerformanceReviewRequest updated)
        {
            var orgId = _tenantService.GetTenantId();
        
            var existingReview = await _context.PerformanceReviews
                .FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId);

            if (existingReview == null)
            {
                throw new Exception("Review not found");
            }

            // =========================
            // UPDATE MAIN REVIEW
            // =========================

            existingReview.EmployeeId = updated.EmployeeId;
            existingReview.ReviewerId = updated.ReviewerId;
            existingReview.ReviewPeriod = updated.ReviewPeriod;
            existingReview.Score = updated.Score;
            existingReview.Summary = updated.Summary;
            existingReview.Status = updated.Status;

            // =========================
            // DELETE OLD CHILDREN
            // =========================

            var oldCompetencies = await _context.ReviewCompetencies
                .Where(x => x.PerformanceReviewId == id)
                .ToListAsync();

            var oldGoals = await _context.ReviewGoals
                .Where(x => x.PerformanceReviewId == id)
                .ToListAsync();

            _context.ReviewCompetencies.RemoveRange(oldCompetencies);
            _context.ReviewGoals.RemoveRange(oldGoals);

            await _context.SaveChangesAsync();

            // =========================
            // ADD NEW COMPETENCIES
            // =========================

            var competencies = updated.Competencies
                .Select(c => new ReviewCompetency
                {
                    Id = Guid.NewGuid(),
                    PerformanceReviewId = id,
                    Label = c.Label,
                    Score = c.Score
                });

            await _context.ReviewCompetencies.AddRangeAsync(competencies);

            // =========================
            // ADD NEW GOALS
            // =========================

            var goals = updated.Goals
                .Select(g => new ReviewGoal
                {
                    Id = Guid.NewGuid(),
                    PerformanceReviewId = id,
                    Title = g.Title
                });

            await _context.ReviewGoals.AddRangeAsync(goals);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteReview(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            var review = await _repository.GetByIdAsync(orgId, id);

            if (review == null)
                throw new Exception("Performance review not found");

            await _repository.DeleteAsync(review);
        }
    }
}
