using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ZucoHR.Application.Interfaces;

namespace ZucoHR.Application.Utilities
{
  

    public class RequirePermissionAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _permission;

        public RequirePermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var accessService = context.HttpContext
                .RequestServices
                .GetRequiredService<IAccessService>();

            var user = context.HttpContext.User;

            var userIdClaim = user.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userId = userIdClaim;

            var hasPermission = await accessService
                .HasPermission(userId, _permission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
