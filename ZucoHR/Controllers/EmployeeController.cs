using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Shared;

namespace ZucoHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _employeeService.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeDto e)
        {
            var validator = new CreateEmployeeValidator();
            var result = await validator.ValidateAsync(e);

            if (!result.IsValid)
                return BadRequest(result.Errors.Select(e => e.ErrorMessage));

            var created = await _employeeService.CreateAsync(e);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
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
        public async Task<IActionResult> Delete(Guid id)
        {
            await _employeeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
