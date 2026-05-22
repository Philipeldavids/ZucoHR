using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Application.Interfaces
{
    public interface IAccessService
    {
        Task<bool> HasPermission(string userId, string permissionCode);
        Task<bool> HasFeature(Guid organizationId, string featureCode);
    }
}
