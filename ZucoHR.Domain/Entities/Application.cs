using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class Application
    {
        public Guid Id { get; set; }
        public Guid JobRequisitionId { get; set; }
        public JobRequisition? JobRequisition { get; set; }
        public string CandidateName { get; set; } = null!;
        public string CandidateEmail { get; set; } = null!;
        public string? ResumeUrl { get; set; }
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Received";
    }
}
