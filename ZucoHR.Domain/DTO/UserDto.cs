using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Domain.DTO
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }

        public string Role { get; set; }

        public List<Permission> Permissions { get; set; }
    }
}
