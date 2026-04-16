using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly TenantContext _context;

        public TenantService(TenantContext context)
        {
            _context = context;
        }

        public Guid GetTenantId()
        {
            if (!_context.HasTenant)
                throw new Exception("Tenant not resolved.");

            return _context.OrganizationId;
        }

        public void SetTenantId(Guid tenantId)
        {
            _context.OrganizationId = tenantId;
        }
    }
}
