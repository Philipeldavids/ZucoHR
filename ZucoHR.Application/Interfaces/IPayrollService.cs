using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Application.Interfaces
{
    public interface IPayrollService
    {
        Task<PayRun> GeneratePayRun(string month, string year);
        Task<IEnumerable<PayRun>> GetAllPayRunsAsync();
        Task<PayRun?> GetPayRunAsync(Guid id);

        Task<PaginatedResponse<Payslip>> GetAllPaySlips(int page = 1, int pageSize = 20);
        Task ApprovePayRunAsync(Guid id);
        Task<IEnumerable<Payslip>> GetEmployeePayslipsAsync(Guid employeeId);
        Task<Payslip?> GetPayslipAsync(Guid id);
        Task<byte[]>
GeneratePayslipPdfAsync(Payslip payroll);
    }
}
