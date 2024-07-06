using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.AspNetCore.SignalR;
using Clipess.DBClient.Infrastructure;
using Clipess.DBClient.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clipess.DBClient.Repositories
{
    public class EFNotificationRepository : INotificationRepository
    {
        private readonly IHubContext<SignalServer> _hubContext;
        private readonly EFDbContext _context;

        public EFNotificationRepository(IHubContext<SignalServer> hubContext, EFDbContext dbContext)
        {
            _hubContext = hubContext;
            _context = dbContext;
        }

        public void CreateNotification(UserNotification notification)
        {
            var createdNotification = new UserNotification
            {
                UserId = notification.UserId,
                NotificationId = notification.NotificationId,
                IsRead = false,
                
            };
            _context.UserNotifications.Add(createdNotification);
            _context.SaveChanges();
        }

        public List<string> GetNotifications(int userId)
        {
            var userNotifications = _context.UserNotifications
                                            .Where(n => n.UserId == userId && !n.IsRead)
                                            .ToList();

            var tableNotifications = _context.Notifications.ToList();

            var relevantNotifications = new List<string>();

            foreach (var userNotify in userNotifications)
            {
                foreach (var notification in tableNotifications)
                {
                    if (userNotify.NotificationId == notification.NotificationId)
                    {
                        relevantNotifications.Add(notification.NotificationText);
                    }
                }
            }
            return relevantNotifications;
        }

        public List<string> GetNotify(int notificationId)
        {
            var notifications = _context.Notifications
                                         .Where(e => e.NotificationId == notificationId)
                                         .Select(e => e.NotificationText)
                                         .ToList();
            return notifications;
        }

        public void ReadNotification(int userId)
        {
            var notifications = _context.UserNotifications
                                        .Where(n => n.UserId == userId && !n.IsRead)
                                        .ToList();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.UtcNow;
                _context.UserNotifications.Update(notification);
                _context.SaveChanges();
            }
        }
    }
}
