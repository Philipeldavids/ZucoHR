using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Domain.DTO
{
    public class ExpenseRequest
    {
       
        public Guid EmployeeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }
        public string? Category { get; set; } // Travel, Meals, Office, etc.

        public string Status { get; set; } = "Pending";

        public IFormFile? Receipt { get; set; }

        public DateTime Date { get; set; }

        
    }
}
