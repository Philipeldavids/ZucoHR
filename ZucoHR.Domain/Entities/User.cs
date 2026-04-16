using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class User : IdentityUser
    {


        public Guid OrganizationId { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "Employee"; // Admin, HR, Manager, Finance
        public Guid EmployeeId { get; set; } = Guid.NewGuid();
        
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
    }
}
