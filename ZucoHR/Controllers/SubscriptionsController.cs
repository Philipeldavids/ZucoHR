using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ITenantService _tenantService;
        private readonly ZucoHrDbContext _context;
        public SubscriptionController(ZucoHrDbContext context, ITenantService tenantService,
            ISubscriptionService subscriptionService
        )
        {

            _context = context;
            _subscriptionService = subscriptionService;
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubscriptions()
        {
            var organizationId =
                _tenantService.GetTenantId();

            var result =
                await _subscriptionService
                    .GetOrganizationSubscriptions(organizationId);

            return Ok(result);
        }
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus( )
        {
            var organizationId = _tenantService.GetTenantId();

            var now = DateTime.UtcNow;

            var subscription =
                await _context.OrgSubscription
                    .Include(x => x.Plan)
                    .FirstOrDefaultAsync(x =>
                        x.OrganizationId == organizationId &&
                        x.IsActive &&
                        x.PaymentConfirmed &&
                        x.StartDate <= now &&
                        x.EndDate >= now
                    );

            if (subscription == null)
            {
                return Ok(new
                {
                    active = false
                });
            }

            return Ok(new
            {
                active = true,
                plan = subscription.Plan.Name,
                endDate = subscription.EndDate
            });
        }
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSubscription()
        {
            var organizationId =
               _tenantService.GetTenantId();

            var result =
                await _subscriptionService
                    .GetActiveSubscription(organizationId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubscription(
            CreateSubscriptionDto dto
        )
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Email)
                 ?? User.FindFirst(ClaimTypes.Email);

            if (claim == null)
                throw new UnauthorizedAccessException("User Email claim missing");
            var email = claim.Value;

            //var organizationId =
            //     _tenantService.GetTenantId();

            var result =
                await _subscriptionService
                    .CreateSubscription(email, dto);

            return Ok(result);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelSubscription(int id)
        {
            var result =
                await _subscriptionService
                    .CancelSubscription(id);

            return Ok(result);
        }
    }
}
    
