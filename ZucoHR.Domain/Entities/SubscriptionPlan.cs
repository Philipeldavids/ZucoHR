using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class SubscriptionPlan
    {
        public Guid Id { get; set; }

        public string Name { get; set; } // Basic, Pro, Enterprise
        public decimal Price { get; set; }

        public ICollection<PlanFeature> Features { get; set; }
    }
}
