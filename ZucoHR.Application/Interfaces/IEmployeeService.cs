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
    public interface IEmployeeService
    {
        Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, Guid orgId);
        Task<Employee?> GetByIdAsync(Guid id, Guid orgId);
        Task<Employee> CreateAsync(EmployeeDto e);
        Task UpdateAsync(Guid id, EmployeeDto dto);
        Task DeleteAsync(Guid id);
    }
}
