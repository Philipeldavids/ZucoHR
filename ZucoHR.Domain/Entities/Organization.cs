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
        public string Domain { get; set; }  // optional
        public string SubscriptionPlan { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
