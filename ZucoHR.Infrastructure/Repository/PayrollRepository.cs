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
    public class PayrollRepository : IPayrollRepository
    {
        private readonly ZucoHrDbContext _context;

        public PayrollRepository(ZucoHrDbContext context)
        {
            _context = context;
        }

        // ---------- PayRun ----------
        public async Task<PayRun> AddPayRunAsync(PayRun payRun)
        {
            await _context.PayRuns.AddAsync(payRun);
            return payRun;
        }

        public async Task<PayRun?> GetPayRunByIdAsync(Guid id)
        {
            return await _context.PayRuns
                .Include(p => p.Payslips)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PayRun>> GetAllPayRunsAsync()
        {
            return await _context.PayRuns
                .Include(p => p.Payslips)
                .OrderByDescending(p => p.PeriodStart)
                .ToListAsync();
        }

        public async Task UpdatePayRunAsync(PayRun payRun)
        {
            _context.PayRuns.Update(payRun);
            await Task.CompletedTask;
        }

        // ---------- Payslip ----------
        public async Task<Payslip> AddPayslipAsync(Payslip payslip)
        {
            await _context.Payslips.AddAsync(payslip);
            return payslip;
        }

        public async Task<Payslip?> GetPayslipByIdAsync(Guid id)
        {
            return await _context.Payslips
                .Include(p => p.Employee)
                .Include(p => p.PayRun)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payslip>> GetPayslipsByPayRunAsync(Guid payRunId)
        {
            return await _context.Payslips
                .Where(p => p.PayRunId == payRunId)
                .Include(p => p.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payslip>> GetPayslipsByEmployeeAsync(Guid employeeId)
        {
            return await _context.Payslips
                .Where(p => p.EmployeeId == employeeId)
                .Include(p => p.PayRun)
                .ToListAsync();
        }

        public async Task UpdatePayslipAsync(Payslip payslip)
        {
            _context.Payslips.Update(payslip);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
