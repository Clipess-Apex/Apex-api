

using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string? TelephoneNo { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set;}
        public bool Deleted { get; set;}
    }
}
