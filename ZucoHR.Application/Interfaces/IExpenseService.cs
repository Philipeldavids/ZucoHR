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
    public interface IExpenseService
    {
        Task<PaginatedResponse<Expense>> GetAllExpenses(int page, int pageSize);
        Task<List<Expense>> GetEmployeeExpenses(Guid employeeId);
        Task<Expense?> GetExpense(Guid id);

        Task CreateExpense(ExpenseRequest request);
        Task UpdateExpense(Guid id, ExpenseRequest request);

        Task ApproveExpense(Guid id, Guid approverId);
        Task RejectExpense(Guid id,string reason, Guid approverId);

        Task DeleteExpense(Guid id);
    }
}
