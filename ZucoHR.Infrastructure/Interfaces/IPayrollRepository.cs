using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IPayrollRepository
    {
        // PayRuns
        Task<PayRun> AddPayRunAsync(PayRun payRun);
        Task<PayRun?> GetPayRunByIdAsync(Guid id);
        Task<IEnumerable<PayRun>> GetAllPayRunsAsync();
        Task UpdatePayRunAsync(PayRun payRun);

        // Payslips
        Task<Payslip> AddPayslipAsync(Payslip payslip);
        Task<Payslip?> GetPayslipByIdAsync(Guid id);
        Task<IEnumerable<Payslip>> GetPayslipsByPayRunAsync(Guid payRunId);
        Task<IEnumerable<Payslip>> GetPayslipsByEmployeeAsync(Guid employeeId);
        Task UpdatePayslipAsync(Payslip payslip);

        Task SaveChangesAsync();
    }
}
