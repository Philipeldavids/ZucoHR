using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class EmployeeShift
    {
        public Guid EmployeeId { get; set;  }
        public Guid ShiftId { get; set; }
    }
}
