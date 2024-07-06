using Clipess.DBClient.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
