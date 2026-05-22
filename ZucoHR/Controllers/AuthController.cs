using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZucoHR.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ZucoHR.Application.Interfaces;
    using ZucoHR.Domain.DTO;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ✅ Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(
                    request.Email,                   
                    request.Name,
                     request.Password,                    
                    request.OrganizationName,
                    request.Role

                );

                return Ok(new
                {
                    message = "User registered successfully",
                    userId = user.Id,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var (accessToken, refreshToken, user, org) = await _authService.SignInAsync(
                    request.Email,
                    request.Password
                );

                return Ok(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = user,
                    Organization = org
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // ✅ Refresh Token
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            try
            {
                var (accessToken, refreshToken) = await _authService.RefreshAsync(
                    request.RefreshToken
                );

                return Ok(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
