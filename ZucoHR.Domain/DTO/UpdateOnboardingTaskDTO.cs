using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class UpdateOnboardingTaskDTO
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public string? Status { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
