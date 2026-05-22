using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class ToggleUserStatusRequest
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
