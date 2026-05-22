using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class Feature
    {
        public Guid Id { get; set; }
        public string Code { get; set; } // "PAYROLL", "RECRUITMENT"
    }
}
