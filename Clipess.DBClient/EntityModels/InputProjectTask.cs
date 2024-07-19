using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.EntityModels
{
    public class InputProjectTask
    {
        [Key]
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public int TeamId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProjectId { get; set; }
        public int TaskStatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Assigned { get; set; }
        public DateTime? AssignedDate { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public List<string> SelectedUsers { get; set; }
    }
}
