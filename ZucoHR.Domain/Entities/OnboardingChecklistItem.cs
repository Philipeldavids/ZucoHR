using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class OnboardingChecklistItem
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public string Description { get; set; } = null!;
        public bool Completed { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
    }

}
