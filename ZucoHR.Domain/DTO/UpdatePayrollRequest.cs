using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class UpdatePayrollRequest
    {
        public decimal BasicSalary { get; set; }

        public decimal Allowances { get; set; }

        //public decimal TotalDeductions { get; set; }

        public decimal AnnualRent { get; set; }
        public string Status { get; set; }
    }
}
