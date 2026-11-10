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
        public Guid OrganizationId { get; set; }

        public Guid EmployeeId { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public string Role { get; set; } = "Employee";

        public List<string>? Permissions { get; set; }

        public ICollection<RefreshToken>? RefreshTokens { get; set; }

        public Organization Organization { get; set; } = null!;
    }
}
