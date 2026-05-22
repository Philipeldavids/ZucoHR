using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class OrganizationSubscription
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid PlanId { get; set; }
        public SubscriptionPlan Plan { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
