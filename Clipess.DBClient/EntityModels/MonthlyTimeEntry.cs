using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clipess.DBClient.EntityModels
{
    public class MonthlyTimeEntry
    {
        [Key]
        public int MonthlyTimeEntryId { get; set; }
        public int EmployeeId { get; set; }
        public string Month { get; set; }
        public int AllocatedDuration { get; set; }
        public int? CompletedDuration { get; set; }
        public string? AttendancePdfUrl {  get; set; }

        //Link Tables
        public Employee? Employee { get; set; }
    }
}
