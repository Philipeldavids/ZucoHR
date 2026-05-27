using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class OrganizationSubscription
    {
        public int Id { get; set; }

        public Guid OrganizationId { get; set; }

        public Organization? Organization { get; set;}

        public int SubscriptionId { get; set; }
        public SubscriptionPlan? Plan { get; set; }

        public DateTime StartDate { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public bool PaymentConfirmed { get; set; }         
        public string? PaymentReference { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
