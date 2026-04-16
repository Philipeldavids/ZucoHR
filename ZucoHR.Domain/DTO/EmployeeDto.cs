using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class EmployeeDto
    {
        public string UserId { get; set; }
        public string Department { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime HireDate { get; set; }
        public decimal BaseSalary { get; set; }

        public decimal Allowances { get; set; }
        public string? Position { get; set; }
    }
}
