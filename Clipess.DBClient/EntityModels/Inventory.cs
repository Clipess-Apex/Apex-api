

using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }
        public int InventoryTypeId { get; set; }
        public string InventoryName { get; set; }
        public int? EmployeeId { get; set; }
        public string? ImageUrl { get; set; }
        public string? FileUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        public DateTime? AssignedDate { get; set; }

        //to linq tables
        public Employee? Employee { get; set; }
        public InventoryType InventoryType { get; set; }

    }
}
