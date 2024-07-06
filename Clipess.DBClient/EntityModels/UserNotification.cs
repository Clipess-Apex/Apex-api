using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.EntityModels
{
    public class UserNotification
    {
        [Key]
        public int UserNotificationId { get; set; }
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReadDate { get; set; }
    }
}
