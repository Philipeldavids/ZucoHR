using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class EmployeeBulkDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string EmployeeNumber { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public DateTime StartDate { get; set; }

        public decimal BasicSalary { get; set; }

        public decimal Allowance { get; set; }

        public decimal AnnualRent { get; set; }

        public string Status { get; set; } = "active";

        public Guid? ShiftId { get; set; }

        public string EmploymentType { get; set; }
        public string? Location { get; set; }
    }
}
