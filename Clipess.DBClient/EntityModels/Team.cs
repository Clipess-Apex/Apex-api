
using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set;}
        public bool Deleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
