using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;
namespace Clipess.DBClient.Contracts
{
    public interface IInventoryUserNotificationRepository
    {
        public IQueryable<InventoryUserNotification> GetInventoryNotifications();

        public InventoryUserNotification GetInventoryNotificationById(int NotificationId);
        public void AddInventoryNotification(InventoryUserNotification notification);
        public IQueryable<InventoryUserNotification> GetInventoryNotificationByEmployeeId(int? employeeId);
        public void ReadNotification(InventoryUserNotification notificationRead);


        void SaveChanges();
    }
}
