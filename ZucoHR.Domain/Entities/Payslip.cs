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

        public Guid OrganizationId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid PayRunId { get; set; }

        // Earnings
        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal GrossPay { get; set; }

        // Deductions
        public decimal Pension { get; set; }
        public decimal NHF { get; set; }
        public decimal Tax { get; set; }
        public decimal OtherDeductions { get; set; }

        public decimal TotalDeductions { get; set; }

        // Net
        public decimal NetPay { get; set; }

        public string? PdfUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public PayRun PayRun { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}
