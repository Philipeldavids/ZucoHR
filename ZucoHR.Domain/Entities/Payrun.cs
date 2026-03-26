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
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string Status { get; set; } = "Generated";
        public decimal TotalNet { get; set; }

        public ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
    }
}