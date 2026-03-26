using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Interfaces
{
    public interface IPayrollService
    {
        Task<PayRun> GeneratePayRunAsync(DateTime start, DateTime end);
        Task<IEnumerable<PayRun>> GetAllPayRunsAsync();
        Task<PayRun?> GetPayRunAsync(Guid id);
        Task ApprovePayRunAsync(Guid id);
        Task<IEnumerable<Payslip>> GetEmployeePayslipsAsync(Guid employeeId);
        Task<Payslip?> GetPayslipAsync(Guid id);
    }
}
