using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class ApplyJobRequest
    {
        public Guid JobId { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? CoverLetter { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? PortfolioUrl { get; set; }

        public IFormFile Resume { get; set; } = default!;
    }
}
