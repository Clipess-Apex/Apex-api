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

        public async Task<bool> CreateNotificationsForManagers(int employeeId, int leaveId, string message, List<int> managerIds)
        {
            var notifications = managerIds.Select(managerId => new LeaveNotification
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
            }).ToList();

            DbContext.LeaveNotifications.AddRange(notifications);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public IQueryable <LeaveNotification> GetNotificationsForManager(int sendTo)
        {
            //var notifications = await DbContext.LeaveNotifications
            //    .Include (n => n.Leave)
            //    .Where(n => n.Send_To == sendTo && !n.IsRead && n.Leave.StatusId == 1)
            //    .ToListAsync();

            //// Map to ManagerGetNotification with only Id and Message
            //var managerNotifications = notifications
            //    .Select(n => new ManagerGetNotification(n.LeaveId, n.Message))
            //    .ToList();

            //return managerNotifications;

            var notifications = DbContext.LeaveNotifications.Where(x => x.ManagerId == sendTo);

            var updatedNotifications = notifications.Include(x => x.Leave);

            var correctNotification = updatedNotifications.Where(x => x.Leave.StatusId == 1);

            if(notifications == null)
            {
                return null;
            }

            return correctNotification;
        }




        public async Task<bool> MarkNotificationAsRead(int notificationId)
        {
            var notification = await DbContext.LeaveNotifications.FindAsync(notificationId);
            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;
            notification.Read_at = DateTime.UtcNow; 
            var result = await DbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> CreateNotificationsForEmployee(int employeeId, int leaveId, string message, int managerId)
        {
            try
            {
                var notification = new LeaveNotification
                {
                    EmployeeId = employeeId,
                    LeaveId = leaveId,
                    Message = message,
                    ManagerId = managerId,
                    Send_To = managerId,
                    Send_By = employeeId,
                    Created_at = DateTime.UtcNow,
                    IsRead = false
                };

                await DbContext.LeaveNotifications.AddAsync(notification);
                await DbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



    }
}
