using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;

namespace ZucoHR.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<PublicCompanyProfileDto>
    GetPublicCompanyProfileAsync(string slug);
    }
}
