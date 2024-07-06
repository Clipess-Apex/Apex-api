using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class UserNotification
    {
        [Key]
        public int UserNotificationId { get; set; }
        public int NotificationId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReadDate { get; set; }
    }
}
