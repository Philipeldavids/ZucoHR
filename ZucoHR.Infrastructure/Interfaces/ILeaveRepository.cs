using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface ILeaveRepository
    {
        Task<LeaveRequest> AddAsync(LeaveRequest request);
        Task<LeaveRequest?> GetByIdAsync(Guid id, Guid orgId);
        Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(Guid employeeId, Guid orgId);
        Task UpdateAsync(LeaveRequest request);
    }
}
