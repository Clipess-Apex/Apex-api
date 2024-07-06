using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class PasswordChange
    {
        [Required]
        public string CompanyEmail { get; set; }
        [Required]
        public string CurrentPassword { get; set;}
        [Required]
        public string NewPassword { get; set;}
    }
}
