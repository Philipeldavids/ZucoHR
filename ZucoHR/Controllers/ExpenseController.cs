using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;
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
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllExpenses());
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployee(Guid employeeId)
        {
            return Ok(await _service.GetEmployeeExpenses(employeeId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.GetExpense(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Expense expense)
        {
            await _service.CreateExpense(expense);
            return Ok(new { message = "Expense submitted" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Expense expense)
        {
            await _service.UpdateExpense(id, expense);
            return Ok(new { message = "Expense updated" });
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromQuery] Guid approverId)
        {
            await _service.ApproveExpense(id, approverId);
            return Ok(new { message = "Expense approved" });
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromQuery] Guid approverId)
        {
            await _service.RejectExpense(id, approverId);
            return Ok(new { message = "Expense rejected" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteExpense(id);
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
