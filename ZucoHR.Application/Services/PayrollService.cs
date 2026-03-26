using Microsoft.Extensions.Logging;
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
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<PayrollService> _logger;

        public PayrollService(
            IPayrollRepository payrollRepository,
            IEmployeeRepository employeeRepository,
            ILogger<PayrollService> logger)
        {
            _payrollRepository = payrollRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<PayRun> GeneratePayRunAsync(DateTime start, DateTime end)
        {
            var employees = await _employeeRepository.GetPagedAsync(1, int.MaxValue);
            var payRun = new PayRun
            {
                Id = Guid.NewGuid(),
                PeriodStart = start,
                PeriodEnd = end,
                Status = "Generated"
            };

            await _payrollRepository.AddPayRunAsync(payRun);

            decimal totalNet = 0m;
            foreach (var emp in employees.Items)
            {
                decimal netPay = 5000m; // Replace with your actual logic
                totalNet += netPay;

                var payslip = new Payslip
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = emp.Id,
                    PayRunId = payRun.Id,
                    NetPay = netPay
                };

                await _payrollRepository.AddPayslipAsync(payslip);
            }

            payRun.TotalNet = totalNet;
            await _payrollRepository.UpdatePayRunAsync(payRun);
            await _payrollRepository.SaveChangesAsync();

            _logger.LogInformation("Generated PayRun {Id} for {Start} - {End}", payRun.Id, start, end);
            return payRun;
        }

        public async Task<IEnumerable<PayRun>> GetAllPayRunsAsync() =>
            await _payrollRepository.GetAllPayRunsAsync();

        public async Task<PayRun?> GetPayRunAsync(Guid id) =>
            await _payrollRepository.GetPayRunByIdAsync(id);

        public async Task ApprovePayRunAsync(Guid id)
        {
            var payRun = await _payrollRepository.GetPayRunByIdAsync(id);
            if (payRun == null) throw new InvalidOperationException("PayRun not found.");

            payRun.Status = "Approved";
            await _payrollRepository.UpdatePayRunAsync(payRun);
            await _payrollRepository.SaveChangesAsync();

            _logger.LogInformation("Approved PayRun {Id}", id);
        }

        public async Task<IEnumerable<Payslip>> GetEmployeePayslipsAsync(Guid employeeId) =>
            await _payrollRepository.GetPayslipsByEmployeeAsync(employeeId);

        public async Task<Payslip?> GetPayslipAsync(Guid id) =>
            await _payrollRepository.GetPayslipByIdAsync(id);
    }
}
