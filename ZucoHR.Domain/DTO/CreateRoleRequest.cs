using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class CreateRoleRequest
    {
        public string Name { get; set; }
        //public string? Description { get; set; }
        public List<Guid> PermissionIds { get; set; }
    }
}
