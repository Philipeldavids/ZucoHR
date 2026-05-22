using Azure.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Utilities;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Infrastructure.Repository;
using ZucoHR.Shared;

namespace ZucoHR.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        //private readonly IWebHostEnvironment _env;
        private readonly IExpenseRepository _repository;
        private readonly ITenantService _tenantService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;
        public ExpenseService(IEmailService emailService, IExpenseRepository repository,IEmployeeRepository employeeRepository,ICloudinaryService cloudinaryService, ITenantService tenantService)
        {
            _repository = repository;
            _tenantService = tenantService;
            _cloudinaryService = cloudinaryService;
            _employeeRepository = employeeRepository;
            _emailService = emailService;
        }

        public async Task<PaginatedResponse<Expense>> GetAllExpenses(int page= 1, int pageSize= 100)
        {
            var orgId = _tenantService.GetTenantId();

            return await _repository.GetAllAsync(orgId, page, pageSize);
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

        public async Task CreateExpense(ExpenseRequest request)
        {
            var orgId = _tenantService.GetTenantId();
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, orgId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            Expense expense = new Expense();

            expense.Id = Guid.NewGuid();
            expense.OrganizationId = _tenantService.GetTenantId();
            expense.Status = request.Status;
            expense.Title = request.Title;
            expense.EmployeeId = request.EmployeeId;
            expense.EmployeeName = employee.FirstName+ " "+ employee.LastName; 
            expense.Description = request.Description;
            expense.Currency = request.Currency;
            expense.Date = request.Date;
            expense.Category = request.Category;
            expense.Amount = request.Amount;

            try
            {
                if (request.Receipt != null)
                {
                    var getReceiptUrl = await _cloudinaryService.UploadFileAsync(request.Receipt);
                    expense.ReceiptUrl = getReceiptUrl.SecureUrl.ToString();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            expense.CreatedAt = DateTime.UtcNow;

            await _repository.AddAsync(expense);
        }

        public async Task UpdateExpense(Guid id, ExpenseRequest updated)
        {
            var orgId = _tenantService.GetTenantId();
            var existing = await _repository.GetByIdAsync(orgId,id);

            if (existing == null)
                throw new Exception("Expense not found");

            if (existing.Status != "Pending")
                throw new Exception("Only pending expenses can be updated");

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.Amount = updated.Amount;
            existing.Category = updated.Category;
            existing.Currency = updated.Currency;
            existing.Date = updated.Date;
            existing.EmployeeId = updated.EmployeeId;

            try
            {
                if (updated.Receipt != null)
                {
                    var getReceiptUrl = await _cloudinaryService.UploadFileAsync(updated.Receipt);
                    existing.ReceiptUrl = getReceiptUrl.SecureUrl.ToString();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            await _repository.UpdateAsync(existing);
        }

        public async Task ApproveExpense(Guid id, Guid approverId)
        {
            var orgId = _tenantService.GetTenantId();
            var expense = await _repository.GetByIdAsync(orgId,id);

            if (expense == null)
                throw new Exception("Expense not found");

            expense.Status = "Approved";
            expense.ApprovedAt = DateTime.UtcNow;
            expense.ApprovedBy = approverId;

            await _repository.UpdateAsync(expense);
        }

        public async Task RejectExpense(Guid id, string Reason, Guid approverId)
        {
            var orgId = _tenantService.GetTenantId();
            var expense = await _repository.GetByIdAsync(orgId,id);

            if (expense == null)
                throw new Exception("Expense not found");

            expense.Status = "Rejected";
            expense.Reason = Reason;
            expense.RejectedAt = DateTime.UtcNow;
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
