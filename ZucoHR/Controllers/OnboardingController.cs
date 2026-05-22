using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Interfaces;

namespace ZucoHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService _service;
        private readonly ITenantService _tenantService;

        public OnboardingController(
            IOnboardingService service,
            ITenantService tenantService
        )
        {
            _service = service;
            _tenantService = tenantService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 100)
        {
            var orgId = _tenantService.GetTenantId();
            var result =
                await _service.GetAllAsync(orgId, page, pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> GetById(
            Guid id
        )
        {
            var result =
                await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> Create(
            [FromBody]
        CreateOnboardingTaskDTO dto
        )
        {
            var result =
                await _service.CreateAsync(dto);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody]
        UpdateOnboardingTaskDTO dto
        )
        {
            var updated =
                await _service.UpdateAsync(
                    id,
                    dto
                );

            if (!updated)
                return NotFound();

            return Ok(new
            {
                message = "Task updated"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> Delete(
            Guid id
        )
        {
            var deleted =
                await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return Ok(new
            {
                message = "Task deleted"
            });
        }
    }
}