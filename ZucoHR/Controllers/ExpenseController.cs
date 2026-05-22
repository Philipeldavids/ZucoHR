using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _service;

        public ExpenseController(IExpenseService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] int page=1, [FromQuery]int pageSize = 100)
        {
            return Ok(await _service.GetAllExpenses(page, pageSize));
        }

        [HttpGet("employee/{employeeId}")]
        [Authorize]
        public async Task<IActionResult> GetByEmployee(Guid employeeId)
        {
            return Ok(await _service.GetEmployeeExpenses(employeeId));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.GetExpense(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm]ExpenseRequest request)
        {
            try
            {
                var employeeId = Guid.Parse(User.FindFirst("employeeId")?.Value ?? throw new UnauthorizedAccessException());
                request.EmployeeId = employeeId;
                await _service.CreateExpense(request);
                return Ok(new { message = "Expense submitted" });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id,[FromForm]ExpenseRequest request)
        {
            try
            {
                var employeeId = Guid.Parse(User.FindFirst("employeeId")?.Value ?? throw new UnauthorizedAccessException());
                request.EmployeeId = employeeId;
                await _service.UpdateExpense(id, request);
                return Ok(new { message = "Expense updated" });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin, Manager, HR Manager")]
        public async Task<IActionResult> Approve(string id)
        {

            try
            {
                var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirst(ClaimTypes.NameIdentifier);

                if (claim == null)
                    throw new UnauthorizedAccessException("Approval ID claim missing");
                var approverId = Guid.Parse(claim.Value);
                await _service.ApproveExpense(Guid.Parse(id), approverId);
                return Ok(new { message = "Expense approved" });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "Admin, Manager, HR Manager")]
        public async Task<IActionResult> Reject(string id, [FromBody] RejectExpenseRequest request)
        {
            try
            {
                var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirst(ClaimTypes.NameIdentifier);

                if (claim == null)
                    throw new UnauthorizedAccessException("Approval ID claim missing");
                var approverId = Guid.Parse(claim.Value);
                await _service.RejectExpense(Guid.Parse(id), request.Reason, approverId);
                return Ok(new { message = "Expense rejected" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Manager, HR Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteExpense(Guid.Parse(id));
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
