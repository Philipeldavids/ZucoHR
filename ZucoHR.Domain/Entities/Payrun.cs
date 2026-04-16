using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class PayRun
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public string Status { get; set; } = "Draft"; // Draft, Approved, Paid

        public decimal TotalGross { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNet { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
    }
}