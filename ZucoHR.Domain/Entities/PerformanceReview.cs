using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class PerformanceReview
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public Guid ReviewerId { get; set; }

        public Guid OrganizationId { get; set; }

        public string ReviewPeriod { get; set; } = null!;

        public string Summary { get; set; } = null!;

        public int Score { get; set; } // 1 - 5

        public string Status { get; set; } = "Draft";

        public DateTime CreatedAt { get; set; }

        // NEW
        //[JsonIgnore]
        public ICollection<ReviewCompetency> Competencies { get; set; }
            = new List<ReviewCompetency>();

        //[JsonIgnore]
        public ICollection<ReviewGoal> Goals { get; set; }
            = new List<ReviewGoal>();
    }
}
