using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class TimeEntryType
    {
        [Key]
        public int TypeId { get; set; }
        public string TimeEntryName { get; set; }
        public string TimeEntryDescription { get; set;}
        
    }
}
