using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class OnboardingTask
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }

        public Guid OnboardingId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public Guid AssignedTo { get; set; } // HR, IT, Manager

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
