using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;

namespace ZucoHR.Application.Services
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;

        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            try
            {
                var user = context.User;

                if (user.Identity != null && user.Identity.IsAuthenticated)
                {
                    var orgClaim = user.Claims.FirstOrDefault(x => x.Type == "OrganizationId");

                    if (orgClaim != null && Guid.TryParse(orgClaim.Value, out var orgId))
                    {
                        tenantService.SetTenantId(orgId);
                        _logger.LogDebug($"Tenant resolved: {orgId}");
                    }
                    else
                    {
                        _logger.LogWarning("OrganizationId claim missing for authenticated user.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving tenant.");
            }

            await _next(context);
        }
    }
}
