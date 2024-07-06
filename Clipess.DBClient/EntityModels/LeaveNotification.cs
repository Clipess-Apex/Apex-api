using System;
using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class LeaveNotification
    {
        [Key]
        public int NotificationId { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveId { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        public DateTime? Read_at { get; set; }
        public int ManagerId { get; set; }
        public int Send_To { get; set; }
        public int Send_By { get; set; }
    }

    public class ManagerGetNotification
    {
        public int EmployeeId { get; set; }
        public int LeaveId { get; set; }
        public string? Message { get; set; }
        public DateTime Created_at { get; set; }
    }

}
