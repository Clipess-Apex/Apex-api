using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class InventoryUserNotification
    {
        [Key]
        public int UserNotificationId { get; set; }
        
        public string Notification { get; set; }

        public int EmployeeId { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsRead { get; set;}

       

    }
}
