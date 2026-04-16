using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class OnboardingDocument
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid OnboardingId { get; set; }

        public string DocumentName { get; set; }
        public string FileUrl { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}
