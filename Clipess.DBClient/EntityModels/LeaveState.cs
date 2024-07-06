

using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class LeaveState
    {
        [Key]
        public int LeaveStatusId { get; set; }
        public string LeaveStatus { get; set;}
        public bool Deleted { get; set;}


    }
}
