using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        //public string Domain { get; set; }  // optional
        //public string SubscriptionPlan { get; set; }
        public string CurrencyCode { get; set; } = "NGN";

        public string CurrencySymbol { get; set; } = "₦";

        public string Timezone { get; set; } = "Africa/Lagos";
        public string Slug { get; set; } // unique (for subdomain later)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrganizationSubscription> OrgSubscriptions { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
