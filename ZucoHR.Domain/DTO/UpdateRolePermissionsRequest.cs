using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class UpdateRolePermissionsRequest
    {
        public string RoleId { get; set; }
        public List<string> PermissionIds { get; set; }
    }
}
