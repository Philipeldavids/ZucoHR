using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class PlanFeature
    {
        public Guid PlanId { get; set; }
        public SubscriptionPlan Plan { get; set; }

        public Guid FeatureId { get; set; }
        public Feature Feature { get; set; }
    }
}
