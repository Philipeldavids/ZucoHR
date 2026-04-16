using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;

namespace ZucoHR.Infrastructure.Repository
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ZucoHrDbContext _context;

        public ExpenseRepository(ZucoHrDbContext context)
        {
            _context = context;
        }

        public async Task<List<Expense>> GetAllAsync(Guid orgId)
        {
            return await _context.Expenses
                .AsNoTracking()
                .Where(x => x.OrganizationId == orgId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Expense>> GetByEmployeeAsync(Guid orgId, Guid employeeId)
        {
            return await _context.Expenses
                .AsNoTracking()
                .Where(x => x.EmployeeId == employeeId && x.OrganizationId == orgId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Expense?> GetByIdAsync(Guid orgId, Guid id)
        {
            return await _context.Expenses
                .AsNoTracking()
                .Where(x => x.OrganizationId == orgId)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Expense expense)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
    }
}
