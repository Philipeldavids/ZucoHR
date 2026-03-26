using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Interfaces
{
    public interface ILeaveService
    {
        Task<LeaveRequest> RequestLeaveAsync(Guid employeeId, DateTime start, DateTime end, string reason);
        Task ApproveAsync(Guid leaveId, Guid approverId);
        Task RejectAsync(Guid leaveId, Guid approverId, string comment);
    }
}
