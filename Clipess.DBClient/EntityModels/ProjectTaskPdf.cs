using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.EntityModels
{
    public class ProjectTaskPdf
    {
        [Key]
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Project { get; set; }
        public string EndDate { get; set; }
        public string TaskStatus { get; set; }
        public string CreatedDate { get; set; }
        public string AssignedDate { get; set; }
    }
}
