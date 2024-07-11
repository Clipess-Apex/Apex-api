using Azure.Core;
using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;


namespace Clipess.DBClient.Repositories
{
    public class EFInventoryUserNotificationRepository : IInventoryUserNotificationRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFInventoryUserNotificationRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }


        public IQueryable<InventoryUserNotification> GetInventoryNotifications()
        {
            return DbContext.InventoryUserNotifications;
              
        }
       


        public InventoryUserNotification GetInventoryNotificationById(int NotificationId)
        {
            return DbContext.InventoryUserNotifications.FirstOrDefault(i => i.UserNotificationId == NotificationId);


        }

        public void AddInventoryNotification(InventoryUserNotification notification)
        {
            DbContext.InventoryUserNotifications.Add(notification);
        }

       

       public IQueryable <InventoryUserNotification> GetInventoryNotificationByEmployeeId(int? employeeId)
        {
            IQueryable <InventoryUserNotification> query = DbContext.InventoryUserNotifications;

            if (employeeId.HasValue)
            {
                query = query.Where(notification => notification.EmployeeId == employeeId);
            }

            return query;
        }



        public void ReadNotification(InventoryUserNotification notificationRead)
        {
            DbContext.InventoryUserNotifications.Update(notificationRead);
        }

        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }
    }
}
