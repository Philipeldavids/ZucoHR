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
        Task<PayRun?> GetPayRunByIdAsync(Guid id, Guid OrgId);
        Task<IEnumerable<PayRun>> GetAllPayRunsAsync(Guid OrgId);
        Task UpdatePayRunAsync(PayRun payRun);

        // Payslips
        Task<Payslip> AddPayslipAsync(Payslip payslip);
        Task<Payslip?> GetPayslipByIdAsync(Guid id, Guid OrgId);
        Task<IEnumerable<Payslip>> GetPayslipsByPayRunAsync(Guid payRunId, Guid OrgId);
        Task<IEnumerable<Payslip>> GetPayslipsByEmployeeAsync(Guid employeeId, Guid OrgId);
        Task UpdatePayslipAsync(Payslip payslip);

        Task SaveChangesAsync();
    }
}
