using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Interfaces;

namespace ZucoHR.Application.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly IPerformanceRepository _repository;
        private readonly ITenantService _tenantService;

        public PerformanceService(
            IPerformanceRepository repository,
            ITenantService tenantService)
        {
            _repository = repository;
            _tenantService = tenantService;
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

        public async Task CreateReview(PerformanceReview review)
        {
            review.Id = Guid.NewGuid();
            review.OrganizationId = _tenantService.GetTenantId();
            review.CreatedAt = DateTime.UtcNow;

            await _repository.CreateAsync(review);
        }

        public async Task UpdateReview(Guid id, PerformanceReview updated)
        {
            var orgId = _tenantService.GetTenantId();
            var existing = await _repository.GetByIdAsync(orgId, id);

            if (existing == null)
                throw new Exception("Performance review not found");

            existing.Score = updated.Score;
            existing.Summary = updated.Summary;
            existing.ReviewPeriod = updated.ReviewPeriod;

            await _repository.UpdateAsync(existing);
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
