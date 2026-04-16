using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Shared;

namespace ZucoHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;
        private readonly ILogger<PayrollController> _logger;
        private readonly ITenantService _tenantService;
        public PayrollController(IPayrollService payrollService, ILogger<PayrollController> logger, ITenantService tenantService)
        {
            _payrollService = payrollService;
            _logger = logger;
            _tenantService = tenantService;
        }

        // POST: api/payroll/generate
       

            [HttpPost("run")]
            public async Task<IActionResult> RunPayroll(DateTime start, DateTime end)
            {
                var result = await _payrollService.GeneratePayRun(start, end);
                return Ok(result);
            }


            // GET: api/payroll
            [HttpGet]
        public async Task<IActionResult> GetPayRuns([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var OrgId = _tenantService.GetTenantId();   
            var allPayRuns = await _payrollService.GetAllPayRunsAsync();
            var total = allPayRuns.Count();

            var paged = allPayRuns
                .OrderByDescending(p => p.PeriodStart)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PayRunResponseDto
                {
                    Id = p.Id,
                    PeriodStart = p.PeriodStart,
                    PeriodEnd = p.PeriodEnd,
                    Status = p.Status,
                    TotalNet = p.TotalNet
                });

            _logger.LogInformation("Retrieved {Count} pay runs for page {Page}", paged.Count(), page);

            return Ok(new
            {
                total,
                page,
                pageSize,
                items = paged
            });
        }

        // GET: api/payroll/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPayRunById(Guid id)
        {
            var payRun = await _payrollService.GetPayRunAsync(id);
            if (payRun == null)
            {
                _logger.LogWarning("Pay run {Id} not found", id);
                return NotFound();
            }

            return Ok(new PayRunResponseDto
            {
                Id = payRun.Id,
                PeriodStart = payRun.PeriodStart,
                PeriodEnd = payRun.PeriodEnd,
                Status = payRun.Status,
                TotalNet = payRun.TotalNet
            });
        }

        // PUT: api/payroll/{id}/approve
        [HttpPut("{id:guid}/approve")]
        public async Task<IActionResult> ApprovePayRun(Guid id)
        {
            try
            {
                await _payrollService.ApprovePayRunAsync(id);
                _logger.LogInformation("Pay run {Id} approved successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving pay run {Id}", id);
                return StatusCode(500, "Error approving pay run");
            }
        }

        // GET: api/payroll/{employeeId}/payslips
        [HttpGet("employee/{employeeId:guid}/payslips")]
        public async Task<IActionResult> GetEmployeePayslips(Guid employeeId)
        {
            var payslips = await _payrollService.GetEmployeePayslipsAsync(employeeId);

            var response = payslips.Select(p => new PayslipResponseDto
            {
                Id = p.Id,
                EmployeeId = p.EmployeeId,
                PayRunId = p.PayRunId,
                NetPay = p.NetPay,
                PdfUrl = p.PdfUrl
            });

            _logger.LogInformation("Fetched {Count} payslips for employee {EmployeeId}", response.Count(), employeeId);
            return Ok(response);
        }

        // GET: api/payroll/payslip/{id}
        [HttpGet("payslip/{id:guid}")]
        public async Task<IActionResult> GetPayslipById(Guid id)
        {
            var payslip = await _payrollService.GetPayslipAsync(id);
            if (payslip == null)
            {
                _logger.LogWarning("Payslip {Id} not found", id);
                return NotFound();
            }

            return Ok(new PayslipResponseDto
            {
                Id = payslip.Id,
                EmployeeId = payslip.EmployeeId,
                PayRunId = payslip.PayRunId,
                NetPay = payslip.NetPay,
                PdfUrl = payslip.PdfUrl
            });
        }
    }
}
