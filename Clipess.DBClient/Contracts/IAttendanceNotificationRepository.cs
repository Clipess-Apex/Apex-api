
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface IAttendanceNotificationRepository
    {
        int CheckNextMonthMonthlyTimeEntries();
        IQueryable<TimeEntryNotification> GetTimeEntryNotifications(int employeeId);
        Task<List<TimeEntryNotification>> CreateTimeEntryNotifications();
        Task<TimeEntryNotification> HideTimeEntryNotification(int notificationId);
    }
}
