using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.EntityModels
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string NotificationText { get; set; }
       
    }
}
