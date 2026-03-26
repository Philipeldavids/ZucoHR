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
        Task<LeaveRequest?> GetByIdAsync(Guid id);
        Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(Guid employeeId);
        Task UpdateAsync(LeaveRequest request);
    }
}
