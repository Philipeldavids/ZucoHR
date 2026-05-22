using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IPerformanceRepository
    {
        Task<PaginatedResponse<PerformanceReview>> GetReviews(Guid orgId, int page = 1, int pageSize = 10);
        Task<List<PerformanceReview>> GetByEmployeeAsync(Guid orgId, Guid employeeId);
        Task<PerformanceReview> GetByIdAsync(Guid orgId, Guid id);
        Task CreateAsync(PerformanceReview review);
        Task Update(PerformanceReview review);
        Task DeleteAsync(PerformanceReview review);
    }
}
