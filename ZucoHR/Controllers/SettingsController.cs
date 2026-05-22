using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly ZucoHrDbContext _dbContext;
        public SettingsController(ITenantService tenantService, ZucoHrDbContext dbContext)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
        }
        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings(
    UpdateOrganizationSettingsRequest request)
        {
            var orgId = _tenantService.GetTenantId();

            var org = await _dbContext.Organizations
                .FirstOrDefaultAsync(x => x.Id == orgId);

            if (org == null)
                return NotFound();

            org.CurrencyCode = request.CurrencyCode;
            org.CurrencySymbol = request.CurrencySymbol;
            org.Timezone = request.Timezone;

            await _dbContext.SaveChangesAsync();

            return Ok(org);
        }
    }
}
