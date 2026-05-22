using Azure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;

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

        public async Task<PayRun?> GetPayRunByIdAsync(Guid id, Guid orgId)
        {
            return await _context.PayRuns
                .Where(x=>x.OrganizationId == orgId)
                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PayRun>> GetAllPayRunsAsync(Guid OrgId)
        {
            return await _context.PayRuns.
                Where(x=> x.OrganizationId == OrgId)
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

        public async Task<Payslip?> GetPayslipByIdAsync(Guid id, Guid OrgId)
        {
            return await _context.Payslips
                .Where(x=>x.OrganizationId == OrgId)
                .Include(p => p.Employee)                
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payslip>> GetPayslipsByPayRunAsync(Guid payRunId, Guid OrgId)
        {
            return await _context.Payslips
                .Where(p => p.PayRunId == payRunId && p.OrganizationId == OrgId)
                .Include(p => p.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payslip>> GetPayslipsByEmployeeAsync(Guid employeeId, Guid OrgId)
        {
            return await _context.Payslips
                .Where(p => p.EmployeeId == employeeId && p.OrganizationId == OrgId)
                .ToListAsync();
        }
        public async Task<PaginatedResponse<Payslip>> GetAllSlip(Guid orgId, int page=1, int pageSize = 10) 
        {

            var query = _context.Payslips.AsQueryable()
                .Where(x => x.OrganizationId == orgId)
                .Include(x => x.PayRun)
                .Include(x=>x.Employee);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Payslip>
            {
                Data = items,
                Total = totalCount,
                Page = page,
                PageSize = pageSize
            };

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
