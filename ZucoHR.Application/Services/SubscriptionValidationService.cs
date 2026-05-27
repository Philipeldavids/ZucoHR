using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Application.Services
{
    public class SubscriptionValidationService
    {
        private readonly ZucoHrDbContext _context;

        public SubscriptionValidationService(
            ZucoHrDbContext context
        )
        {
            _context = context;
        }

        public async Task<bool> HasActiveSubscription(
            Guid organizationId
        )
        {
            var now = DateTime.UtcNow;

            return await _context.OrgSubscription
                .AnyAsync(x =>
                    x.OrganizationId == organizationId &&
                    x.IsActive &&
                    x.PaymentConfirmed &&
                    x.StartDate <= now &&
                    x.EndDate >= now
                );
        }
    }
}