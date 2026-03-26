using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class JobRequisition
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        public bool IsOpen { get; set; } = true;
    }
}
