using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;

namespace ZucoHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaystackController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IConfiguration _configuration;
        public PaystackController(IConfiguration configuration, ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
            _configuration = configuration;
        }
        private static string ComputeSha512Hash(
    string rawData,
    string secret
)
        {
            var encoding = new UTF8Encoding();

            byte[] keyBytes =
                encoding.GetBytes(secret);

            byte[] messageBytes =
                encoding.GetBytes(rawData);

            using var hmac =
                new HMACSHA512(keyBytes);

            byte[] hashMessage =
                hmac.ComputeHash(messageBytes);

            return BitConverter
                .ToString(hashMessage)
                .Replace("-", "")
                .ToLower();
        }

        [Authorize]
        [HttpPost("verify-payment")]
        public async Task<IActionResult> Webhook([FromBody] VerifyPaymentRequest request)
        {
            try
            {
                await _subscriptionService
                    .VerifyAndActivateSubscription(request.Reference);
                return Ok();
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
            
        }
    
    }
}
