using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class NotificationContoller : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationContoller(INotificationService service)
        {
            _service = service;
        }

        [HttpPost("create")]

        public async Task<IActionResult> CreateAsync(string userId, string title, string message, string type = "Info")
        {
            try
            {
                var res = await _service.CreateAsync(userId, title, message, type);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]

        public async Task<IActionResult> GetUserNotifications()
        {
            try
            {
                var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
                 ?? User.FindFirst(ClaimTypes.NameIdentifier);

                if (claim == null)
                    throw new UnauthorizedAccessException("USER ID claim missing");
                var userId = claim.Value;
                var res = await _service.GetUserNotifications(userId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/{id}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                var res = await _service.MarkAsRead(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
