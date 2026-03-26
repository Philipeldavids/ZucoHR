using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public enum ExpenseStatus { Submitted, Approved, Rejected, Paid }
    public class ExpenseClaim
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public ExpenseStatus Status { get; set; } = ExpenseStatus.Submitted;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
