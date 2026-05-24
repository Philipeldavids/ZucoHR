using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;

namespace ZucoHR.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _service;
        private readonly ITenantService _tenantService;
        public AttendanceController(
            ITenantService tenantService,
            IAttendanceService service)
        {
            _service = service;
            _tenantService = tenantService;
        }

        [HttpPost("clock-in")]
        public async Task<IActionResult> ClockIn(
            [FromBody] ClockInDto dto)
        {
            var employeeId =
                Guid.Parse(User.FindFirst("employeeId")!.Value);
            var orgId = _tenantService.GetTenantId();
            await _service.ClockInAsync(orgId, employeeId, dto);

            return Ok(new
            {
                message = "Clock in successful"
            });
        }

        [HttpPost("clock-out")]
        public async Task<IActionResult> ClockOut([FromBody]ClockOutDto dto)
        {
            var employeeId =
                Guid.Parse(User.FindFirst("employeeId")!.Value);
            var orgId = _tenantService.GetTenantId();
            await _service.ClockOutAsync(orgId, employeeId, dto);

            return Ok(new
            {
                message = "Clock out successful"
            });
        }

        [HttpGet("today")]
        public async Task<IActionResult> Today()
        {
            var employeeId =
                Guid.Parse(User.FindFirst("employeeId")!.Value);
            var orgId = _tenantService.GetTenantId();
            var attendance =
                await _service.GetTodayAttendance(orgId,employeeId);

            return Ok(attendance);
        }
        [HttpGet("my-attendance")]
        public async Task<IActionResult> EmployeeAttendance(
        )
        {
            var orgId = _tenantService.GetTenantId();
            var employeeId =
                Guid.Parse(User.FindFirst("employeeId")!.Value);
            var result =
                await _service.GetEmployeeAttendanceAsync(
                    employeeId, orgId);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> AllAttendance()
        {
            var orgId = _tenantService.GetTenantId();
            var result =
                await _service.GetAllAttendanceAsync(orgId);

            return Ok(result);
        }
    }
}
