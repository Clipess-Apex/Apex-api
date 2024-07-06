using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.AspNetCore.SignalR;
using Clipess.DBClient.Infrastructure;
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
                EmployeeId = notification.EmployeeId,
                NotificationId = notification.NotificationId,
                IsRead = false,
                CreatedDate = DateTime.UtcNow

            };
            _context.UserNotifications.Add(createdNotification);
            _context.SaveChanges();

        } 

        public  List<String> GetNotifications(int EmployeeId)
        {
            var userNotification = _context.UserNotifications
                                        .Where(n => n.EmployeeId == EmployeeId && !n.IsRead).ToList();

            var tableNotification = _context.Notifications.ToList();

            var relevantNotification = new List<String>();

            
            foreach (UserNotification userNotify in userNotification)
            {
                foreach(Notification notification in tableNotification)
                {
                    if ((userNotify.NotificationId) == (notification.NotificationId))
                    {
                        relevantNotification.Add(notification.NotificationText);
                    }
                }
                

            }
            return relevantNotification;

        }

        public  List<String> GetNotify(int notificatioId)
        {
            var notification =  _context.Notifications
                 .Where(e => e.NotificationId == notificatioId)
                 .Select(e => e.NotificationText)
                 .ToList();
            return notification;


        }



        public void ReadNotification(int EmployeeId)
        {
            var notifications = _context.UserNotifications
                                        .Where(n => n.EmployeeId == EmployeeId  && !n.IsRead).ToList();

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
