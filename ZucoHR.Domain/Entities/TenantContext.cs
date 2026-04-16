using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class TenantContext
    {
        public Guid OrganizationId { get; set; }
        public bool HasTenant => OrganizationId != Guid.Empty;
    }
}
