using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class ClockInDto
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string LocationAddress { get; set; }

        public string? DeviceInfo { get; set; }

        public string? WorkMode { get; set; }
    }
}
