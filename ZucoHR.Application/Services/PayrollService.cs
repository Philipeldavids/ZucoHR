using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Utilities;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;

namespace ZucoHR.Application.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<PayrollService> _logger; 
        private readonly ITenantService _tenantService;
        private readonly ZucoHrDbContext _context;
        public PayrollService(
            IPayrollRepository payrollRepository,
            IEmployeeRepository employeeRepository,
            ILogger<PayrollService> logger, ITenantService tenantService, ZucoHrDbContext context)
        {
            _payrollRepository = payrollRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
            _tenantService = tenantService;
            _context = context;
        }
        


            public async Task<PayRun> GeneratePayRun(DateTime start, DateTime end)
            {
                var orgId = _tenantService.GetTenantId();

                var employees = await _context.Employees.ToListAsync();

                var payRun = new PayRun
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = orgId,
                    PeriodStart = start,
                    PeriodEnd = end,
                    Status = "Generated"
                };

                foreach (var emp in employees)
                {
                    var (gross, pension, nhf, tax, net) =
                        PayrollCalculator.CalculateMonthly(emp.BasicSalary, emp.Allowances);

                    var payslip = new Payslip
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = orgId,
                        EmployeeId = emp.Id,
                        PayRunId = payRun.Id,

                        BasicSalary = emp.BasicSalary,
                        Allowances = emp.Allowances,
                        GrossPay = gross,

                        Pension = pension,
                        NHF = nhf,
                        Tax = tax,
                        OtherDeductions = 0,

                        TotalDeductions = pension + nhf + tax,
                        NetPay = net
                    };

                    payRun.TotalGross += gross;
                    payRun.TotalDeductions += payslip.TotalDeductions;
                    payRun.TotalNet += net;

                    payRun.Payslips.Add(payslip);
                }

                await _context.PayRuns.AddAsync(payRun);
                await _context.SaveChangesAsync();

                return payRun;
            }

            //public async Task<List<Payslip>> GetPayslips(Guid payRunId)
            //{
            //    return await _context.Payslips
            //        .Where(x => x.PayRunId == payRunId)
            //        .Include(x => x.Employee)
            //        .ToListAsync();
            //}
        
        public async Task<IEnumerable<PayRun>> GetAllPayRunsAsync()
        {
            var OrgId = _tenantService.GetTenantId();
            return await _payrollRepository.GetAllPayRunsAsync(OrgId);
        }


        public async Task<PayRun?> GetPayRunAsync(Guid id) {
            var OrgId = _tenantService.GetTenantId();
            return await _payrollRepository.GetPayRunByIdAsync(id, OrgId);
        }
            

        public async Task ApprovePayRunAsync(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            var payRun = await _payrollRepository.GetPayRunByIdAsync(id, orgId);
            if (payRun == null) throw new InvalidOperationException("PayRun not found.");

            payRun.Status = "Approved";
            await _payrollRepository.UpdatePayRunAsync(payRun);
            await _payrollRepository.SaveChangesAsync();

            _logger.LogInformation("Approved PayRun {Id}", id);
        }

        public async Task<IEnumerable<Payslip>> GetEmployeePayslipsAsync(Guid employeeId)
        {
            var orgId = _tenantService.GetTenantId();
            return await _payrollRepository.GetPayslipsByEmployeeAsync(employeeId, orgId);
        }
           

        public async Task<Payslip?> GetPayslipAsync(Guid id)
        {
            var orgId= _tenantService.GetTenantId();
            return await _payrollRepository.GetPayslipByIdAsync(id,orgId);
        }
            
    }
}
