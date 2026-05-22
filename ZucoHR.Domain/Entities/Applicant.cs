using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    //public enum ApplicationStatus
    //{
    //    Applied = 0,
    //    Shortlisted = 1,
    //    Interviewed = 2,
    //    Offered = 3,
    //    Hired = 4,
    //    Rejected = 5
    //}

    public class Applicant
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid JobPostId { get; set; }

        public string? JobPostTitle { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? ResumeUrl { get; set; }

        public string? Stage { get; set; } = "Applied";

        public string? PortfolioUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? CoverLetter { get; set; }
        public DateTime AppliedAt { get; set; }

        public string? Notes { get; set; }
    }
}
