using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;

namespace ZucoHR.Application.Interfaces
{
    public interface ISuperAdminService
    {
        Task<SuperAdminDashboardDto> GetDashboardAsync();

        Task<List<SuperAdminCompanyDto>> GetCompaniesAsync();

        Task<List<SuperAdminUserDto>> GetUsersAsync();

        Task<bool> ResetUserPasswordAsync(
            string userId,
            string newPassword);

        Task<bool> DeleteUserAsync(string userId);
    }
}
