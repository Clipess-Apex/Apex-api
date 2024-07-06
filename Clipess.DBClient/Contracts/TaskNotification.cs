using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.Contracts
{
    public class TaskNotification
    {
        public int notificationId { get; set; }
        public List<int> EmployeeId { get; set; }
        
    }
}
