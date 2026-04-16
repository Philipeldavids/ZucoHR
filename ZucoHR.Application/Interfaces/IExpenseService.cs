using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllExpenses();
        Task<List<Expense>> GetEmployeeExpenses(Guid employeeId);
        Task<Expense?> GetExpense(Guid id);

        Task CreateExpense(Expense expense);
        Task UpdateExpense(Guid id, Expense expense);

        Task ApproveExpense(Guid id, Guid approverId);
        Task RejectExpense(Guid id, Guid approverId);

        Task DeleteExpense(Guid id);
    }
}
