using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITenantService _tenantService;
        private readonly ZucoHrDbContext _context;
        private readonly IEmailService _emailService;
        public EmployeesController(IEmailService emailService, ZucoHrDbContext context, IEmployeeService employeeService, ITenantService tenantService)
        {
            _employeeService = employeeService;
            _tenantService = tenantService;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkUpload(
    [FromBody]
    List<EmployeeBulkDto> employees)
        {
            if (employees == null ||
                !employees.Any())
            {
                return BadRequest(
                    "No employees supplied");
            }
            var orgId = _tenantService.GetTenantId();
            User newuser = new User();
            var entities =
                employees.Select(e =>
                    new Employee
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = orgId,
                        FirstName =
                            e.FirstName,
                        UserId = newuser.Id,
                        LastName =
                            e.LastName,
                        EmploymentType = e.EmploymentType,
                        Email = e.Email,

                        PhoneNumber =
                            e.PhoneNumber,

                        Department =
                            e.Department,

                        Position =
                            e.Position,
                        EmployeeNumber = e.EmployeeNumber,

                        StartDate =
                            e.StartDate,
                        BasicSalary = e.BasicSalary,
                        Allowance = e.Allowance,
                        AnnualRent = e.AnnualRent,

                        Location = e.Location,
                        Status =
                            e.Status ?? "active"
                    });
            
           

            foreach (var entity in entities)
            {
                var password = PasswordGenerator.Generate(12);

                

                newuser.UserName = entity.Email;
                newuser.Email = entity.Email;
                newuser.Name = entity.FirstName + " " + entity.LastName;
                newuser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                newuser.OrganizationId = orgId;
                newuser.EmployeeId = entity.Id;
                newuser.Role = "Employee";

                await _context.Users.AddAsync(newuser);
                    await _emailService.SendEmailAsync(
    new EmailRequest
    {
        To = entity.Email,
        Subject = "Welcome to ZucoHR",
        Body = EmailTemplates.WelcomeEmployee(
            entity.FirstName,
            "ZucoHR",
            entity.Email,
            password

        )
    }
);


            }

            await _context.Employees
               .AddRangeAsync(entities);

            await _context.SaveChangesAsync();
            return Ok(new
            {
                message =
                    "Employees uploaded successfully"
            });
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin, HR, Manager,HR Manager")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var OrgID = _tenantService.GetTenantId();
            var result = await _employeeService.GetPagedAsync(page, pageSize, OrgID);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var OrgID = _tenantService.GetTenantId();
            var employee = await _employeeService.GetByIdAsync(id, OrgID);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Roles ="Admin, HR, HR Manager")]
        //[RequirePermission("EMPLOYEE_CREATE")]
        public async Task<IActionResult> Create([FromBody] EmployeeDto e)
        {
            try
            {
                var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
                 ?? User.FindFirst(ClaimTypes.NameIdentifier);

                if (claim == null)
                    throw new UnauthorizedAccessException("User ID claim missing");
                var userId = claim.Value;

                var validator = new CreateEmployeeValidator();
                var result = await validator.ValidateAsync(e);

                if (!result.IsValid)
                    return BadRequest(result.Errors.Select(e => e.ErrorMessage));


                var created = await _employeeService.CreateAsync(userId, e);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeDto e)
        {
            var validator = new CreateEmployeeValidator();
            var result = await validator.ValidateAsync(e);

            if (!result.IsValid)
                return BadRequest(result.Errors.Select(e => e.ErrorMessage));

            await _employeeService.UpdateAsync(id, e);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, HR, HR Manager")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _employeeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
