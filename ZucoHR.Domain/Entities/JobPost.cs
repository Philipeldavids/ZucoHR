using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class JobPost
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }

        public Organization? Organization { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string? Slug { get; set; }

        public string? Type { get; set; }

        public string Status { get; set; }

        public List<string> Requirements { get; set; }

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public int? Applicants { get; set; }

        public string? Department { get; set; }
        public string? Location { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
