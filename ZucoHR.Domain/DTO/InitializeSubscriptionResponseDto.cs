using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class InitializeSubscriptionResponseDto
    {
        public string AuthorizationUrl { get; set; }

        //public string Reference { get; set; }

        public int SubscriptionId { get; set; }
    }
    
}
