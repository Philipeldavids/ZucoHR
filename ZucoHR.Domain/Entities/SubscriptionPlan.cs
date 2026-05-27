using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }

        public string Name { get; set; } // Basic, Pro, Enterprise
        public decimal Price { get; set; }

        public ICollection<PlanFeature> Features { get; set; } = new List<PlanFeature>();
        public ICollection<OrganizationSubscription> OrganizationSubscriptions { get; set; } = new List<OrganizationSubscription>();
    }
}
