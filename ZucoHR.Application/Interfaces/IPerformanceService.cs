using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Application.Interfaces
{
    public interface IPerformanceService
    {
        Task<PaginatedResponse<PerformanceReview>> GetReviews(int page = 1, int pageSize = 10);
        Task<List<PerformanceReview>> GetEmployeeReviews(Guid employeeId);
        Task<PerformanceReview?> GetReview(Guid id);
        Task CreateReview(PerformanceReviewRequest reviewRequest);
        Task UpdateReview(Guid id, PerformanceReviewRequest review);
        Task DeleteReview(Guid id);
    }
}
