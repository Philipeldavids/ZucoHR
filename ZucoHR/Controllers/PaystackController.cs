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
        public PaystackController(IConfiguration configuration,ISubscriptionService subscriptionService)
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
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            using var reader =
                new StreamReader(Request.Body);

            var body =
                await reader.ReadToEndAsync();

            var signature =
                Request.Headers["x-paystack-signature"];

            var secret =
                _configuration["Paystack:SecretKey"];

            var hash =
                ComputeSha512Hash(body, secret);

            if (hash != signature)
                return Unauthorized();

            dynamic payload =
                JsonConvert.DeserializeObject(body);

            if (
                payload.@event ==
                "charge.success"
            )
            {
                string reference =
                    payload.data.reference;

                await _subscriptionService
                    .VerifyAndActivateSubscription(
                        reference
                    );
            }

            return Ok();
        }
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializePayment(
    InitializeSubscriptionPaymentDto dto
)
        {
            var result =
                await _subscriptionService
                    .InitializePayment(dto);

            return Ok(result);
        }
    }
}
