using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZucoHR.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using ZucoHR.Application.Interfaces;
    using ZucoHR.Domain.DTO;
    using ZucoHR.Domain.Entities;
    using ZucoHR.Infrastructure.Data;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        //private readonly UserManager<User> _userManager;
        private readonly ZucoHrDbContext _context;
        public AuthController(ZucoHrDbContext context, IAuthService authService)
        {
            _authService = authService;
            _context = context;
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

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
    [FromBody] ChangePasswordDto dto)
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)
                 ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new UnauthorizedAccessException("User ID claim missing");
            var userId = claim.Value;
            

            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var user = await _context.Users
       .FirstOrDefaultAsync(x =>
           x.Id == userId
       );

            if (user == null)
                return NotFound("User not found");
            // verify current password
            bool validPassword =
                BCrypt.Net.BCrypt.Verify(
                    dto.CurrentPassword,
                    user.PasswordHash
                );

            if (!validPassword)
            {
                return BadRequest(
                    "Incorrect password"
                );
            }

            // hash new password
            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.NewPassword
                );

            await _context.SaveChangesAsync();

            return Ok(
                "Password changed successfully"
            );
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
