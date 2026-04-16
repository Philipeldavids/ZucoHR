using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Interfaces
{
    public interface IPerformanceService
    {
        Task<List<PerformanceReview>> GetEmployeeReviews(Guid employeeId);
        Task<PerformanceReview?> GetReview(Guid id);
        Task CreateReview(PerformanceReview review);
        Task UpdateReview(Guid id, PerformanceReview review);
        Task DeleteReview(Guid id);
    }
}
