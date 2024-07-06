

using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class InventoryType
    {
        [Key]
       public int InventoryTypeId {  get; set; }

       public string InventoryTypeName { get; set; }


        public string? ReportUrl { get; set; }   

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public int? DeletedBy { get; set; }




    }
}

