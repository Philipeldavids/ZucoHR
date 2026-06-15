using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;

namespace ZucoHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService) 
        { 
            _emailService = emailService;
        }
        [AllowAnonymous]
        [HttpPost("book-demo")]
        public async Task<IActionResult> BookDemo(
    [FromBody] BookDemoDto dto)
        {
            await _emailService.SendBookDemoEmail(dto);

            return Ok(new
            {
                Message = "Demo request submitted."
            });
        }
    }
}
