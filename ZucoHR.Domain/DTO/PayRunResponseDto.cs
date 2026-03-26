using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class PayRunResponseDto
    {
        public Guid Id { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalNet { get; set; }
    }
}
