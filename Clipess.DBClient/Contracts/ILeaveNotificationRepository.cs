using Clipess.DBClient.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clipess.DBClient.Contracts
{
    public interface ILeaveNotificationRepository
    {
        Task<bool> CreateNotification(int employeeId, int leaveId, string message, int managerId);
        Task<List<ManagerGetNotification>> GetNotificationsForManager(int sendTo);
        Task<bool> MarkNotificationAsRead(int notificationId);
    }
}
