using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IPerformanceRepository
    {
        Task<List<PerformanceReview>> GetByEmployeeAsync(Guid orgId, Guid employeeId);
        Task<PerformanceReview> GetByIdAsync(Guid orgId, Guid id);
        Task CreateAsync(PerformanceReview review);
        Task UpdateAsync(PerformanceReview review);
        Task DeleteAsync(PerformanceReview review);
    }
}
