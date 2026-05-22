using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class UserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public Role Role { get; set; } = new Role();
    }
}
