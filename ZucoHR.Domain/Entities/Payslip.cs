using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class Payslip
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid PayRunId { get; set; }
        public decimal NetPay { get; set; }
        public string? PdfUrl { get; set; }

        public PayRun PayRun { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}
