using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class PerformanceReview
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Employee? Employee { get; set; }
        public Guid ReviewerId { get; set; }
        public string ReviewPeriod { get; set; }
        public string Summary { get; set; } = null!;
        public int Score { get; set; } // 1-5

        public DateTime CreatedAt { get; set; }
    }
}
