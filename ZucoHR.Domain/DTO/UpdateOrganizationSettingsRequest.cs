using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class UpdateOrganizationSettingsRequest
    {
        public string CurrencyCode { get; set; } = "NGN";

        public string CurrencySymbol { get; set; } = "₦";

        public string Timezone { get; set; } = "Africa/Lagos";
    }
}
