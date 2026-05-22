using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Infrastructure.Interfaces
{
    public interface IOrganizationRepository
    {

        Task<List<Organization>> GetOrganization();
        Task<Organization> GetOrganizationByid(Guid Id);
    }
}
