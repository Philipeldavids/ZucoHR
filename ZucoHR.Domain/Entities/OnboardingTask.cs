using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{ 

    public class OnboardingTask
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }

        //public Guid OnboardingId { get; set; }

      
        public string Description { get; set; }

        public Guid AssignedTo { get; set; } // HR, IT, Manager

        //public bool IsCompleted { get; set; }
        [Required]
        public string Category { get; set; } = "other";

        [Required]
        public string Status { get; set; } = "pending";

        public DateTime DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
