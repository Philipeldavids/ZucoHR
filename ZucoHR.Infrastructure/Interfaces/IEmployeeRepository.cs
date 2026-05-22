using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(Guid id, Guid orgId);
        Task<PaginatedResponse<Employee>> GetPagedAsync(int page, int pageSize, Guid orgId);
        Task AddAsync(Employee e);
        Task UpdateAsync(Employee e);
        Task DeleteAsync(Employee e);
    }
}
