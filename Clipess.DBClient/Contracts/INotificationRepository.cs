using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface INotificationRepository
    {
        void CreateNotification(UserNotification notification);
        void ReadNotification(int userNotificationId);
        List<String> GetNotifications(int userId);
        List<String> GetNotify(int notificationId);
    }
}
