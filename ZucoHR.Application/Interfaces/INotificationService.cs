using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Services;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Interfaces
{
    public interface INotificationService
    {
        Task<bool> CreateAsync(string userId, string title, string message, string type = "Info");
        Task<List<Notification>> GetUserNotifications(string userId);
        Task<bool> MarkAsRead(Guid Id);
    }
}
