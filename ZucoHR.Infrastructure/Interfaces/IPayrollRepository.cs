using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

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

        Task<PaginatedResponse<Payslip>> GetAllSlip(Guid orgId, int page = 1, int pageSize = 20);
        Task<Payslip?> GetPayslipByIdAsync(Guid id, Guid OrgId);
        Task<IEnumerable<Payslip>> GetPayslipsByPayRunAsync(Guid payRunId, Guid OrgId);
        Task<IEnumerable<Payslip>> GetPayslipsByEmployeeAsync(Guid employeeId, Guid OrgId);
        Task UpdatePayslipAsync(Payslip payslip);

        Task SaveChangesAsync();
    }
}
