using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Clipess.DBClient.EntityModels
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string NIC { get; set; }
        [Required]
        [EmailAddress]
        public string PersonalEmail { get; set; }
        [Required]
        [EmailAddress]
        public string CompanyEmail { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string Address { get; set; }
        public string Designation { get; set; }
        public string? ImageUrl { get; set; }
        public string? FileUrl { get; set; }
        public string? EmergencyContactPersonName { get; set; }
        public string? EmergencyContactPersonMobileNumber { get; set; }
        public string? EmergencyContactPersonAddress { get; set; }
        public int ReportingEmployeeID { get; set; }
        public int EmployeeTypeID { get; set; }
        public int RoleID { get; set; }
        public int DepartmentID { get; set; }
        public int MaritalStatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }
    }
}
