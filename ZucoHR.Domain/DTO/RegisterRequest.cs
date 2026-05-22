using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }

        public string OrganizationName { get; set; }
        public string Role { get; set; } = "Admin";

        
    }
}
