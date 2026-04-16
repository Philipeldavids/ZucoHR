using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class Onboarding
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }

        public Guid ApplicantId { get; set; } // from Recruitment
        public Guid? EmployeeId { get; set; } // set after completion

        public string Status { get; set; } // Pending, InProgress, Completed

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

}
