using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.EntityModels
{
    public class TaskReportRequest
    {
        public List<int> TaskId { get; set; }
        public int EmployeeID { get; set; }
    }
}
