using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Domain.DTO
{
    public class AuthResponse
    {
        public User User { get; set; }

        public Organization Organization { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
