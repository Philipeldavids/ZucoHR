using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Utilities
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;

        public TokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("employeeId", user.EmployeeId.ToString() ?? string.Empty),
                new Claim("OrganizationId", user.OrganizationId.ToString())
            };
            var jwt = _configuration.GetSection("Jwt");
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: authClaims,
                        expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["AccessTokenExpiryMinutes"])),
                        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );


            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;


        }
    }
}
