using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class BookDemoDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Company { get; set; }

        public string Employees { get; set; }

        public DateTime PreferredDate { get; set; }

        public string PreferredTime { get; set; }

        public string Country { get; set; }

        public string Message { get; set; }
    }
}
