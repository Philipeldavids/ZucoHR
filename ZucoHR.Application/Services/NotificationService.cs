using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ZucoHrDbContext _ctx;
        private readonly ITenantService _tenantService;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(
            ZucoHrDbContext ctx, 
            ITenantService tenantService,
            IHubContext<NotificationHub> hub)
        {
            _ctx = ctx;
            _tenantService = tenantService;
            _hub = hub;
        }

        public async Task<bool> CreateAsync(string userId, string title, string message, string type = "Info")
        {
            var orgId = _tenantService.GetTenantId();
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                OrganizationId = orgId
            };

            _ctx.Notifications.Add(notification);
            await _ctx.SaveChangesAsync();

            await _hub.Clients.Group(userId)
        .SendAsync("ReceiveNotification", new
        {
            notification.Id,
            notification.Title,
            notification.Message,
            notification.CreatedAt
        });
            return true;
        }

        public async Task<List<Notification>> GetUserNotifications(string userId)
        {
            var orgId = _tenantService.GetTenantId();
            return await _ctx.Notifications
                  .Where(n => n.UserId == userId && n.OrganizationId == orgId)
                  .OrderByDescending(n => n.CreatedAt)
                  .Take(20)
                  .ToListAsync();
        }

        public async Task<bool> MarkAsRead(Guid id)
        {
            var n = await _ctx.Notifications.FindAsync(id);
            if (n == null) return false;

            n.IsRead = true;
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
