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
        
        Task<User> RegisterAsync(string email, string password, string role = "Employee");
        Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken);

        Task<(string accessToken, string refreshToken)> SignInAsync(string email, string password);
    }
}
