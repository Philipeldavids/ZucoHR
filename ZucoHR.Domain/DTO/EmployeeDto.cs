using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class EmployeeDto
    {
        //public string UserId { get; set; }
        public string? Department { get; set; }
        public string? EmployeeNumber { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime StartDate { get; set; }
        public decimal BasicSalary { get; set; }

        public decimal Allowance { get; set; }

        public decimal AnnualRent { get; set; }
        public string? Position { get; set; }

        public string? Location { get; set; }
        public string Status { get; set; }

        //public string Avatar { get; set; }
        //public string ManagerId { get; set; }
    }
}
