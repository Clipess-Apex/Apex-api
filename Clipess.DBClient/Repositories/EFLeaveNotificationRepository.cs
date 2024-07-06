using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Clipess.DBClient.Repositories
{
    public class EFLeaveNotificationRepository : ILeaveNotificationRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFLeaveNotificationRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<bool> CreateNotification(int employeeId, int leaveId, string message, int managerId)
        {
            var notification = new LeaveNotification
            {
                EmployeeId = employeeId,
                LeaveId = leaveId,
                Message = message,
                ManagerId = managerId,
                IsRead = false,
                Created_at = DateTime.Now,
                Updated_at = null,
                Read_at = null,
                Send_To = managerId,
                Send_By = employeeId
            };

            DbContext.LeaveNotifications.Add(notification);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<ManagerGetNotification>> GetNotificationsForManager(int sendTo)
        {
            var notifications = await DbContext.LeaveNotifications
                .Where(n => n.Send_To == sendTo && !n.IsRead == false)
                .ToListAsync();

            return notifications.Select(n => new ManagerGetNotification
            {
                LeaveId = n.LeaveId,
                Message = n.Message,
                EmployeeId = n.EmployeeId,
                Created_at = n.Created_at
            })
            .ToList();
        }


        public async Task<bool> MarkNotificationAsRead(int notificationId)
        {
            var notification = await DbContext.LeaveNotifications.FindAsync(notificationId);

            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;
            notification.Read_at = DateTime.Now;
            DbContext.LeaveNotifications.Update(notification);
            await DbContext.SaveChangesAsync();
            return true;
        }

    }
}
