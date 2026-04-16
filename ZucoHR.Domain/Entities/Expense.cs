using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public enum ExpenseStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class Expense
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid EmployeeId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public decimal Amount { get; set; }
        public string Category { get; set; } // Travel, Meals, Office, etc.

        public ExpenseStatus Status { get; set; }

        public string? ReceiptUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public Guid? ApprovedBy { get; set; }
    }
}
