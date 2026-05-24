using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Utilities;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
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
        private readonly ZucoHrDbContext _context;
        public PayrollController(ZucoHrDbContext context, IPayrollService payrollService, ILogger<PayrollController> logger, ITenantService tenantService)
        {
            _payrollService = payrollService;
            _logger = logger;
            _context = context;
            _tenantService = tenantService;
        }

        [HttpGet("{id}/payslip")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult>
DownloadPayslip(Guid id)
        {
            var payroll =
                await _context.Payslips
                .Include(x=>x.PayRun)
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (payroll == null)
                return NotFound();

            var pdfBytes =
                await _payrollService
                    .GeneratePayslipPdfAsync(payroll);

            return File(
                pdfBytes,
                "application/pdf",
                $"Payslip-{payroll.Employee.FirstName}.pdf"
            );
        }
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult>
UpdatePayrollStatus(
    string id,
    UpdatePayrollStatusRequest request)
        {
            var payroll =
                await _context.Payslips                
                .Include(x => x.PayRun)
                .Include(x=>x.Employee)
                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

            if (payroll == null)
                return NotFound();

            payroll.PayRun.Status =
                request.Status;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> UpdatePayroll(
    string id,
    [FromBody] UpdatePayrollRequest request)
        {
            var payroll =
                await _context.Payslips
                .Include(x=>x.Employee)
                .Include(x => x.PayRun)
                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

            if (payroll == null)
                return NotFound();

            if (request.BasicSalary != null)
                payroll.BasicSalary =
                    request.BasicSalary;

            if (request.Allowances != null)
                payroll.Allowances =
                    request.Allowances;

            //if (request.TotalDeductions != null)
            //    payroll.TotalDeductions =
            //        request.TotalDeductions;
            if (request.AnnualRent!= null)
                payroll.Employee.AnnualRent = request.AnnualRent;

            var (gross, pension, nhf, nhis, tax, rr, net) =
                     PayrollCalculator.CalculateMonthly(
                         request.BasicSalary,
                         request.Allowances,
                         request.AnnualRent
                     );

            payroll.NetPay = net;
                

            if (!string.IsNullOrWhiteSpace(request.Status)
                && payroll.PayRun != null)
            {
                payroll.PayRun.Status =
                    request.Status;
            }
            payroll.GrossPay = gross;

            payroll.Pension = pension;
            payroll.NHF = nhf;
            payroll.NHIS = nhis;
            payroll.RentRelief = rr;
            payroll.Tax = tax;
            payroll.TotalDeductions = pension + nhf + nhis;

            await _context.SaveChangesAsync();

            return Ok(payroll);
        }
        // POST: api/payroll/generate


        [HttpPost("run")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        //[RequirePermission("PAYROLL_RUN")]
        public async Task<IActionResult> RunPayroll(CreatePayrunRequest request)
            {
                var result = await _payrollService.GeneratePayRun(request.Month, request.Year);
                return Ok(result);
            }


            // GET: api/payroll
            [HttpGet]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> GetPayRuns([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var OrgId = _tenantService.GetTenantId();   
            var allPayRuns = await _payrollService.GetAllPayRunsAsync();
            var total = allPayRuns.Count();

            var paged = allPayRuns
                .OrderByDescending(p => p.PeriodStart)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
                
                //Select(p => new PayRunResponseDto
                //{
                //    Id = p.Id,
                //    PeriodStart = p.PeriodStart,
                //    PeriodEnd = p.PeriodEnd,
                //    Status = p.Status,
                //    TotalNet = p.TotalNet
                //});

            _logger.LogInformation("Retrieved {Count} pay runs for page {Page}", paged.Count(), page);

            return Ok(new PaginatedResponse<PayRun>
            {
                Data = paged,
                Total = total,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/payroll/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
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
        [Authorize(Roles = "Admin, HR, HR Manager")]
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
        [Authorize(Roles = "Admin, HR, HR Manager")]
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

        [HttpGet("payslipall")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> GetAllPayslip([FromQuery]int page=1, [FromQuery]int pageSize = 100)
        {
            try
            {
                var res = await _payrollService.GetAllPaySlips(page, pageSize);
                return Ok(res);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
