using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IExpenseRepository
    {
        Task<PaginatedResponse<Expense>> GetAllAsync(Guid orgId, int page, int pageSize);
        Task<List<Expense>> GetByEmployeeAsync(Guid orgId,Guid employeeId);
        Task<Expense?> GetByIdAsync(Guid orgId, Guid id);

        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(Expense expense);
    }
}
