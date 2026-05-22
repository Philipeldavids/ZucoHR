using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Interfaces
{
    public interface IAuthService
    {

        Task<User> RegisterAsync(string email, string name, string password, string role = "Admin", string organizationName = null);
        Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken);

        Task<(string accessToken, string refreshToken, User user, Organization organization)> SignInAsync(string email, string password);
    }
}
