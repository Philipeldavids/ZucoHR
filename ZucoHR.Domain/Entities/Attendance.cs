using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class Attendance
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid OrganizationId { get; set; }

        public Employee Employee { get; set; } = default!;

        public DateTime Date { get; set; }

        public DateTime? ClockInTime { get; set; }

        public DateTime? ClockOutTime { get; set; }

        public string Status { get; set; } = "Present";

        public string? LocationAddress { get; set; }

        //public string? ClockOutLocation { get; set; }

        public double? ClockInLatitude { get; set; }

        public double? ClockInLongitude { get; set; }

        public double? ClockOutLatitude { get; set; }

        public double? ClockOutLongitude { get; set; }

        public int WorkedMinutes { get; set; }

        public bool IsLate { get; set; }

        public bool IsRemote { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}
