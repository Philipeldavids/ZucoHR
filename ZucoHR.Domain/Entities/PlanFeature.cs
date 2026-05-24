using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class PlanFeature
    {
        public int Id { get; set; }
        public SubscriptionPlan Plan { get; set; }
         public int PlanId { get; set; }
        public Guid FeatureId { get; set; }
        public Feature Feature { get; set; }
    }
}
