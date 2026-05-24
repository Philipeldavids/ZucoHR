using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Interfaces
{
    public interface IAttendanceService
    {
        Task ClockInAsync(Guid OrgId,Guid employeeId, ClockInDto dto);

        Task<IEnumerable<Attendance>>
            GetAllAttendanceAsync(Guid OrgId);
        Task<IEnumerable<Attendance>> GetEmployeeAttendanceAsync(Guid employeeId, Guid OrgId);
        Task ClockOutAsync(Guid OrgId,Guid employeeId, ClockOutDto dto);

        Task<Attendance?> GetTodayAttendance(Guid OrgId, Guid employeeId);

    }
}
