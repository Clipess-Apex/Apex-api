using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class TimeEntry
    {
        [Key]
        public int TimeEntryId { get; set; }
        public int EmployeeId { get; set; }
        public int TimeEntryTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Deleted { get; set; }
        public int? Duration { get; set; }
        public string? Description { get; set; }
    }
}
