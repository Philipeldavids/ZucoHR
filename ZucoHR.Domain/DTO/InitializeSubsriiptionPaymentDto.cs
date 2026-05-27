using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class InitializeSubscriptionPaymentDto
    {
        public Guid OrganizationId { get; set; }

        public int PlanId { get; set; }

        public string Email { get; set; }
    }
}
