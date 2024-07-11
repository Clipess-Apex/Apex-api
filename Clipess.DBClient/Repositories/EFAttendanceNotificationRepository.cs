using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Clipess.DBClient.Repositories
{
    public enum NextMonthMonthlyTimeEntries
    {
        RecordsAvailable = 1,
        RecordsUnavailable = 2
    }
    public class EFAttendanceNotificationRepository : IAttendanceNotificationRepository
    {
        public EFDbContext _DbContext { get; set; }
        public EFAttendanceNotificationRepository(EFDbContext dbContext)
        {
            _DbContext = dbContext;
        }



        public int CheckNextMonthMonthlyTimeEntries()
        {
            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime nextMonth;

            if (currentMonth.Month == 12)
            {
                nextMonth = new DateTime(currentMonth.Year + 1, 1, 1);
            }
            else
            {
                nextMonth = currentMonth.AddMonths(1);
            }

            string nextMonthString = nextMonth.ToString("yyyy-MM", CultureInfo.InvariantCulture);
            var monthlyTimeEntries = _DbContext.MonthlyTimeEntries.Where(x => x.Month == nextMonthString).ToList();

            if(monthlyTimeEntries.Count > 0)
            {
                return (int)NextMonthMonthlyTimeEntries.RecordsAvailable;
            }

            return (int)NextMonthMonthlyTimeEntries.RecordsUnavailable;
        }

        public IQueryable<TimeEntryNotification> GetTimeEntryNotifications(int employeeId)
        {
            var timeEntryNotification = _DbContext.TimeEntryNotifications.Where(x => x.IsRead == false && x.ReadBy == employeeId);
            if (timeEntryNotification == null)
            {
                return null;
            }
            return timeEntryNotification;
        }

        public async Task<List<TimeEntryNotification>> CreateTimeEntryNotifications()
        {
            var managerIds = _DbContext.Employees.Where( x =>  x.RoleID == 2 && x.Deleted == false).Select(u => u.EmployeeID).ToList();

            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime nextMonth;
            if (currentMonth.Month == 12)
            {
                nextMonth = new DateTime(currentMonth.Year + 1, 1, 1);
            }
            else
            {
                nextMonth = currentMonth.AddMonths(1);
            }
            string nextMonthString = nextMonth.ToString("yyyy-MM", CultureInfo.InvariantCulture);

            var timeEntryNotifications = new List<TimeEntryNotification>();
            foreach (var x in managerIds)
            {
                var timeEntryNotification = new TimeEntryNotification
                {
                    Month =nextMonthString,
                    Message = $" You have not Created the Monthly Time Entries for {nextMonthString}",
                    ReadBy = x,
                    CreatedDate = DateTime.Now
                };
                timeEntryNotifications.Add(timeEntryNotification);
            }

           _DbContext.TimeEntryNotifications.AddRange(timeEntryNotifications);
           await _DbContext.SaveChangesAsync();

            return timeEntryNotifications;

             
        }

        public async Task<TimeEntryNotification> HideTimeEntryNotification(int notificationId)
        {
            var notification = _DbContext.TimeEntryNotifications.FirstOrDefault(x => x.Id == notificationId);

            if (notification == null) 
            {
                return null;
            }

            notification.IsRead = true;

            _DbContext.SaveChanges();

            return notification;
        }
    }
}
