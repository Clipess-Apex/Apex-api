using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clipess.DBClient.EntityModels
{
    public class DailyTimeEntry
    {
        [Key]
        public int DailyTimeEntryId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public DateTime? LunchIn { get; set; }
        public DateTime? LunchOut { get; set;}
        public int? TotalDuration { get; set; }

        //Linked Tables
        public Employee? Employee { get; set; }

    }
}
