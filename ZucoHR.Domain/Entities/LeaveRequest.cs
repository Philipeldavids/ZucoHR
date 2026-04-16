using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public enum LeaveStatus { Pending, Approved, Rejected, Cancelled }
    public class LeaveRequest
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }

        public Guid OrganizationId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = null!;
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public Guid? ApprovedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

}
