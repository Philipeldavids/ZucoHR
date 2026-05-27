using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class SubscriptionResponseDto
    {
        public int Id { get; set; }

        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int PlanId { get; set; }

        public string PlanName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public bool PaymentConfirmed { get; set; }

      
        public string Status { get; set; }
    }
}