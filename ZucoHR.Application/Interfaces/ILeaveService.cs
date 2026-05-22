using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Application.Interfaces
{
    public interface ILeaveService
    {
        Task<IEnumerable<LeaveRequest>> GetByEmployee(Guid employeeId);
        Task<PaginatedResponse<LeaveRequest>> GetAll(Guid orgId, int page, int pagesize);
        Task<LeaveRequest> RequestLeaveAsync(Guid employeeId, string leaveType, DateTime start, DateTime end, string reason);
        Task ApproveAsync(Guid leaveId, Guid approverId);
        Task RejectAsync(Guid leaveId, Guid approverId, string comment);
    }
}
