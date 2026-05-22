using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class PublicCompanyProfileDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Slug { get; set; }
        public string? Tagline { get; set; }

        public string? Description { get; set; }
        public int? TeamSize { get; set; }
        public int? Countries { get; set; }

    }
}
