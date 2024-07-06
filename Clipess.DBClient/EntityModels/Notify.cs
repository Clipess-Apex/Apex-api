using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.EntityModels
{
    public class Notify
    {
        [Key]
        public int UserNotificationId { get; set; }
        public string NotificationText { get; set; }

 
    }
}
