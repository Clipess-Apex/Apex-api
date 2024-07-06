

using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Clipess.DBClient.EntityModels
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }

        public int EmployeeId { get; set; }

        public int InventoryId { get; set; }

        public int InventoryTypeId { get; set; }

        public string? Message { get; set; }

        public string Inventory { get; set; }

        public bool IsRead { get; set; }

        public bool Deleted { get; set; }

        //public DateTime CreatedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? DeletedDate { get; set; }

        public bool Rejected { get; set; }

        public string? Reason { get; set; }

        //to linq tables
        
        public InventoryType? InventoryType { get; set; }
        public Employee? Employee { get; set; }





    }
}

