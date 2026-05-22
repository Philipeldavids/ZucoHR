using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Utilities;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Shared;

namespace ZucoHR.Application.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<PayrollService> _logger; 
        private readonly ITenantService _tenantService;
        private readonly ZucoHrDbContext _context;
        private readonly IEmailService _emailService;
        public PayrollService(IEmailService emailService,
            IPayrollRepository payrollRepository,
            IEmployeeRepository employeeRepository,
            ILogger<PayrollService> logger, ITenantService tenantService, ZucoHrDbContext context)
        {
            _payrollRepository = payrollRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
            _tenantService = tenantService;
            _context = context;
            _emailService = emailService;
        }



        public async Task<PayRun> GeneratePayRun(string month, string year)
        {
            var orgId = _tenantService.GetTenantId();

            bool exists = await _context.PayRuns.AnyAsync(x =>
                x.OrganizationId == orgId &&
                x.Month == month &&
                x.Year == year);

            if (exists)
                throw new Exception("Payroll already generated for this period.");

            var start = new DateTime();
            var end = start.AddMonths(1).AddDays(-1);

            var employees = await _context.Employees
                .Where(x => x.OrganizationId == orgId)
                .ToListAsync();

            var payRun = new PayRun
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                Month = month,
                Year = year,
                PeriodStart = start,
                PeriodEnd = end,
                Status = "Draft"
            };

            foreach (var emp in employees)
            {
                var (gross, pension, nhf, nhis, tax, rr, net) =
                    PayrollCalculator.CalculateMonthly(
                        emp.BasicSalary,
                        emp.Allowance,
                        emp.AnnualRent
                    );

                var payslip = new Payslip
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = orgId,
                    EmployeeId = emp.Id,
                    PayRunId = payRun.Id,

                    BasicSalary = emp.BasicSalary,
                    Allowances = emp.Allowance,
                    GrossPay = gross,
                    PayRun = payRun,
                    Employee = emp,                   
                    Pension = pension,
                    NHF = nhf,
                    NHIS = nhis,
                    RentRelief = rr,
                    Tax = tax,
                    OtherDeductions = 0,

                    TotalDeductions = pension + nhf + nhis,
                    NetPay = net
                };

                payRun.TotalGross += gross;
                payRun.TotalDeductions += payslip.TotalDeductions;
                payRun.TotalNet += net;

                
                payRun.Payslips.Add(payslip);

                await _emailService.SendEmailAsync(
    new EmailRequest
    {
        To = emp.Email,
        Subject = "Payroll Processed",
        Body = EmailTemplates.PayrollProcessed(
            emp.FirstName,
            payslip.NetPay,
            $"{payslip.PayRun.Month} {payslip.PayRun.Year}"
        )
    }
);
            }

            await _context.PayRuns.AddAsync(payRun);
            await _context.SaveChangesAsync();
            
            return payRun;
        }


    

public async Task<byte[]>
GeneratePayslipPdfAsync(Payslip payroll)
    {
            var org = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == payroll.OrganizationId);

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Header()
                    .Text($"Employee Payslip-{org.Name}")
                    .FontSize(24)
                    .Bold();

                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text($"Employee: {payroll.Employee?.FirstName} {payroll.Employee?.LastName}");

                    col.Item().Text($"Month: {payroll.PayRun?.Month}");

                    col.Item().Text($"Year: {payroll.PayRun?.Year}");

                    col.Item().Text($"Status: {payroll.PayRun?.Status}");

                    col.Item().PaddingVertical(10);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // HEADER
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle)
                                .Text("Description")
                                .Bold();

                            header.Cell().Element(CellStyle)
                                .AlignRight()
                                .Text("Amount")
                                .Bold();
                        });

                        // BASIC
                        table.Cell().Element(CellStyle)
                            .Text("Basic Salary");

                        table.Cell().Element(CellStyle)
                            .AlignRight()
                            .Text($"{payroll.BasicSalary:N2}");

                        // ALLOWANCES
                        table.Cell().Element(CellStyle)
                            .Text("Allowances");

                        table.Cell().Element(CellStyle)
                            .AlignRight()
                            .Text($"{payroll.Allowances:N2}");
                        // PAYE TAX
                        table.Cell().Element(CellStyle)
                            .Text("PAYE Tax");

                        table.Cell().Element(CellStyle)
                            .AlignRight()
                            .Text($"{payroll.Tax:N2}");
                        // DEDUCTIONS
                        table.Cell().Element(CellStyle)
                            .Text("Deductions");

                        table.Cell().Element(CellStyle)
                            .AlignRight()
                            .Text($"{payroll.TotalDeductions:N2}");

                        // NET PAY
                        table.Cell().Element(CellStyle)
                            .Text("Net Pay")
                            .Bold();

                        table.Cell().Element(CellStyle)
                            .AlignRight()
                            .Text($"{payroll.NetPay:N2}")
                            .Bold();
                    });

                    static IContainer CellStyle(
                        IContainer container)
                    {
                        return container
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .PaddingVertical(8)
                            .PaddingHorizontal(4);
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.UtcNow
                            .ToString("dd MMM yyyy HH:mm"));
                    });
            });
        });

        return await Task.FromResult(
            document.GeneratePdf()
        );
    }
    public async Task<IEnumerable<PayRun>> GetAllPayRunsAsync()
        {
            var OrgId = _tenantService.GetTenantId();
            return await _payrollRepository.GetAllPayRunsAsync(OrgId);
        }


        public async Task<PayRun?> GetPayRunAsync(Guid id) {
            var OrgId = _tenantService.GetTenantId();
            return await _payrollRepository.GetPayRunByIdAsync(id, OrgId);
        }
            

        public async Task ApprovePayRunAsync(Guid id)
        {
            var orgId = _tenantService.GetTenantId();
            var payRun = await _payrollRepository.GetPayRunByIdAsync(id, orgId);
            if (payRun == null) throw new InvalidOperationException("PayRun not found.");

            payRun.Status = "Approved";
            await _payrollRepository.UpdatePayRunAsync(payRun);
            await _payrollRepository.SaveChangesAsync();

            _logger.LogInformation("Approved PayRun {Id}", id);
        }

        public async Task<IEnumerable<Payslip>> GetEmployeePayslipsAsync(Guid employeeId)
        {
            var orgId = _tenantService.GetTenantId();
            return await _payrollRepository.GetPayslipsByEmployeeAsync(employeeId, orgId);
        }
           
        public async Task<PaginatedResponse<Payslip>> GetAllPaySlips(int page = 1, int pageSize = 20)
        {
            var orgId = _tenantService.GetTenantId();
            return await _payrollRepository.GetAllSlip(orgId, page, pageSize);

        }

        public async Task<Payslip?> GetPayslipAsync(Guid id)
        {
            var orgId= _tenantService.GetTenantId();
            return await _payrollRepository.GetPayslipByIdAsync(id,orgId);
        }
            
    }
}
