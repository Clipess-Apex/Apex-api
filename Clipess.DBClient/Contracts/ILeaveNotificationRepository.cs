using Clipess.DBClient.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clipess.DBClient.Contracts
{
    public interface ILeaveNotificationRepository
    {
        Task<bool> CreateNotificationsForManagers(int employeeId, int leaveId, string message, List<int> managerIds);
         IQueryable <LeaveNotification> GetNotificationsForManager(int sendTo);
        Task<bool> MarkNotificationAsRead(int notificationId);
        Task<bool> CreateNotificationsForEmployee(int employeeId, int leaveId, string message, int managerId);
    }
}
