using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class Role : IdentityRole
    {
        public Guid Id {  get; set; }
        public string Name {  get; set; }
    }
}
