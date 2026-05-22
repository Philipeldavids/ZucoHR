using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class AssignRoleRequest
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
