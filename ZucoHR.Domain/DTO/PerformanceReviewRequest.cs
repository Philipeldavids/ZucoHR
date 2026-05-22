using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Domain.DTO
{
   
    public class PerformanceReviewRequest
    {
        public Guid EmployeeId { get; set; }
        //public Guid OrganizationId { get; set; }
        public Employee? Employee { get; set; }
        public Guid ReviewerId { get; set; }
        public string ReviewPeriod { get; set; }
        public string Summary { get; set; } = null!;

        public string Status { get; set; } = "Draft";
        public int Score { get; set; } // 1-5

        public List<ReviewCompetencyDTO> Competencies { get; set; }
           = new();

        public List<ReviewGoalDto> Goals { get; set; }
            = new();

    }
}
