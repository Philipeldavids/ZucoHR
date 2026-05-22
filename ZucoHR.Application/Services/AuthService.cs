using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure;
using ZucoHR.Infrastructure.Data;
using ZucoHR.Infrastructure.Interfaces;

namespace ZucoHR.Application.Services
{

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IOrganizationRepository _org;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IConfiguration _config;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ZucoHrDbContext _context;

        public AuthService(IUserRepository users, ZucoHrDbContext context, ITokenGenerator tokenGenerator, IOrganizationRepository org,IRefreshTokenRepository refreshRepo, IConfiguration config)
        {
            _users = users;
            _refreshRepo = refreshRepo;
            _config = config;
            _tokenGenerator = tokenGenerator;
            _context = context;
            _org = org;
        }

        public async Task<User> RegisterAsync(string email, string name, string password, string organizationName = null, string role = "Admin")
        {
            
        
            Organization org;

            if (!string.IsNullOrEmpty(organizationName))
            {
                // Create new company
                org = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = organizationName,
                    Slug = organizationName.Replace(" ", "-").ToLower()
                };

                _context.Organizations.Add(org);

                // Seed roles for this org
                await SeedData.SeedRolesForOrganization(_context, org.Id);
            }
            else
            {
                throw new Exception("Organization name required");
            }

            var existing = await _users.GetByEmailAsync(email);
            if (existing != null) throw new InvalidOperationException("Email already exists");
            var user = new User
            {
                Name = name,
                UserName = email,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                OrganizationId = org.Id
            };
            //await SeedData.SeedRolesForOrganization(_context, user.OrganizationId);
            return await _users.AddAsync(user);
        }

        public async Task<(string accessToken, string refreshToken, User user, Organization organization)> SignInAsync(string email, string password)
        {
            var user = await _users.GetByEmailAsync(email);

            var permission =await _context.UserRoles.SelectMany(ur => ur.Role.RolePermissions)
                            .Select(rp => rp.Permission.Code)
                            .Distinct()
                            .ToListAsync();

            

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");
            if (!user.IsActive)
                throw new UnauthorizedAccessException("User is currently Inactive");
            user.Permissions = permission;
            var org = await _org.GetOrganizationByid(user.OrganizationId);
            if (org == null)
                throw new NullReferenceException("Organization not found");
            var access = _tokenGenerator.GenerateToken(user);
            var refresh = await GenerateAndStoreRefreshToken(user);
            

            return (access, refresh, user, org);
        }

        public async Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken)
        {
            var stored = await _refreshRepo.GetByTokenAsync(refreshToken);
            if (stored == null || stored.IsRevoked || stored.ExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid refresh token");

            var user = stored.User!;
            // revoke old token
            await _refreshRepo.RevokeAsync(stored);

            var access = _tokenGenerator.GenerateToken(user);
            var newRefresh = await GenerateAndStoreRefreshToken(user);

            return (access, newRefresh);
        }

        //private string GenerateAccessToken(User user)
        //{
        //    var jwt = _config.GetSection("Jwt");
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var claims = new[]
        //    {
        //    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        //    new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //    new Claim(ClaimTypes.Role, user.Role),
        //    new Claim("employeeId", user.EmployeeId?.ToString() ?? string.Empty)
        //};
        //    var token = new JwtSecurityToken(
        //        issuer: jwt["Issuer"],
        //        audience: jwt["Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["AccessTokenExpiryMinutes"])),
        //        signingCredentials: creds
        //    );
        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        private async Task<string> GenerateAndStoreRefreshToken(User user)
        {
            var jwt = _config.GetSection("Jwt");
            var refreshExpiryDays = int.Parse(jwt["RefreshTokenExpiryDays"]);
            var tokenStr = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var rt = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = tokenStr,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshExpiryDays),
                IsRevoked = false,
                UserId = Guid.Parse(user.Id),
                User = user
            };
            await _refreshRepo.AddAsync(rt);
            return tokenStr;
        }
    }
}