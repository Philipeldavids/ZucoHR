using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ZucoHrDbContext _context;
        private readonly INotificationService _notificationService;

        public AttendanceService(ZucoHrDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task ClockInAsync(Guid OrgId,
            Guid employeeId,
            ClockInDto dto)
        {
            var employee = await _context.Employees        
        .FirstOrDefaultAsync(x => x.Id == employeeId && x.OrganizationId == OrgId);

            if (employee == null)
                throw new Exception("Employee not found");

            var today = DateTime.UtcNow.Date;

            var existing = await _context.Attendances
                .FirstOrDefaultAsync(x => x.OrganizationId == OrgId &&
                    x.EmployeeId == employeeId &&
                    x.Date == today);

            if (existing != null)
                throw new Exception("Already clocked in");

            var now = DateTime.UtcNow;

            var shiftStart = today.Add(employee.WorkStartTime);

            var lateThreshold =
                shiftStart.AddMinutes(
                    employee.GracePeriodMinutes);

            bool isLate = now > lateThreshold;

            if (isLate)
            {
                await _notificationService.CreateAsync(
    employee.UserId.ToString(),
    "Late Attendance",
    "You checked in late today."
);
            }

            var attendance = new Attendance
            {
                EmployeeId = employeeId,
                Date = today,
                ClockInTime = now,
                ClockInLatitude = dto.Latitude,
                ClockInLongitude = dto.Longitude,
                LocationAddress = dto.LocationAddress,
                OrganizationId = OrgId,
                IsLate = isLate,
                Status = isLate
                    ? "Late"
                    : "Present"
            };

            _context.Attendances.Add(attendance);

            await _context.SaveChangesAsync();
        }


        public async Task ClockOutAsync(Guid OrgId,Guid employeeId, ClockOutDto dto)
        {
            var today = DateTime.UtcNow.Date;

            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(x => x.OrganizationId == OrgId &&
                    x.EmployeeId == employeeId &&
                    x.Date == today);

            if (attendance == null)
                throw new Exception("No active attendance");

            if (attendance.ClockOutTime != null)
                throw new Exception("Already clocked out");

            attendance.ClockOutTime = DateTime.UtcNow;

            attendance.ClockOutLatitude = dto.Latitude;

            attendance.ClockOutLongitude = dto.Longitude;

            //attendance.ClockOutLocation = dto.LocationAddress;

            attendance.WorkedMinutes =
                (int)(attendance.ClockOutTime.Value -
                attendance.ClockInTime!.Value)
                .TotalMinutes;

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Attendance>>
        GetEmployeeAttendanceAsync(Guid employeeId, Guid OrgId)
        {
            return await _context.Attendances
                .Where(x => x.EmployeeId == employeeId && x.OrganizationId == OrgId)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>>
            GetAllAttendanceAsync(Guid OrgId)
        {
            return await _context.Attendances
                .Where(x=>x.OrganizationId == OrgId)
                .Include(x => x.Employee)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public async Task<Attendance?> GetTodayAttendance(Guid OrgId,Guid employeeId)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(x => x.OrganizationId == OrgId &&
                    x.EmployeeId == employeeId &&
                    x.CreatedAt.Date == DateTime.UtcNow.Date);
        }

        //private async Task<Shift?> GetEmployeeShift(Guid employeeId)
        //{
        //    return await _context.Shifts
        //        .Where(x => x.EmployeeId == employeeId)                
        //        .FirstOrDefaultAsync();
        //}
    }
}
