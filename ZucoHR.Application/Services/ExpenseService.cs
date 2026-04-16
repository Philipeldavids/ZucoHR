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
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _repository;
        private readonly ITenantService _tenantService;

        public ExpenseService(IExpenseRepository repository, ITenantService tenantService)
        {
            _repository = repository;
            _tenantService = tenantService;
        }

        public async Task<List<Expense>> GetAllExpenses()
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetAllAsync(orgId);
        }

        public async Task<List<Expense>> GetEmployeeExpenses(Guid employeeId)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetByEmployeeAsync(orgId,employeeId);
        }

        public async Task<Expense?> GetExpense(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            return await _repository.GetByIdAsync(orgId,id);
        }

        public async Task CreateExpense(Expense expense)
        {
            expense.Id = Guid.NewGuid();
            expense.OrganizationId = _tenantService.GetTenantId();
            expense.Status = ExpenseStatus.Pending;
            expense.CreatedAt = DateTime.UtcNow;

            await _repository.AddAsync(expense);
        }

        public async Task UpdateExpense(Guid id, Expense updated)
        {
            var orgId = _tenantService.GetTenantId();
            var existing = await _repository.GetByIdAsync(orgId,id);

            if (existing == null)
                throw new Exception("Expense not found");

            if (existing.Status != ExpenseStatus.Pending)
                throw new Exception("Only pending expenses can be updated");

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.Amount = updated.Amount;
            existing.Category = updated.Category;
            existing.ReceiptUrl = updated.ReceiptUrl;

            await _repository.UpdateAsync(existing);
        }

        public async Task ApproveExpense(Guid id, Guid approverId)
        {
            var orgId = _tenantService.GetTenantId();
            var expense = await _repository.GetByIdAsync(orgId,id);

            if (expense == null)
                throw new Exception("Expense not found");

            expense.Status = ExpenseStatus.Approved;
            expense.ApprovedAt = DateTime.UtcNow;
            expense.ApprovedBy = approverId;

            await _repository.UpdateAsync(expense);
        }

        public async Task RejectExpense(Guid id, Guid approverId)
        {
            var orgId = _tenantService.GetTenantId();
            var expense = await _repository.GetByIdAsync(orgId,id);

            if (expense == null)
                throw new Exception("Expense not found");

            expense.Status = ExpenseStatus.Rejected;
            expense.ApprovedAt = DateTime.UtcNow;
            expense.ApprovedBy = approverId;

            await _repository.UpdateAsync(expense);
        }

        public async Task DeleteExpense(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            var expense = await _repository.GetByIdAsync(orgId, id);

            if (expense == null)
                throw new Exception("Expense not found");

            await _repository.DeleteAsync(expense);
        }
    }
}
