using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace Clipess.DBClient.EntityModels
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string NIC { get; set; }
        public string PersonalEmail { get; set; }
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
        public Department? Department { get; set; }
        public int MaritalStatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }
        public Role? role { get; set; }
    }

    public class LoginModel
    {
        public string CompanyEmail { get; set; }
        public string Password { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string CompanyEmail { get; set; }
    }

    public class ResetPasswordModel
    {
        //public string Token { get; set; }
        public string CompanyEmail { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class EmployeeCountsDto
    {
        public int Total { get; set; }
        public Dictionary<string, int> RoleCounts { get; set; }
    }

    public class DepartmentEmployeeCountDto
    {
        public int DepartmentEmployeeCountTotal { get; set; }
        public Dictionary<string, int> DepartmentCounts { get; set; }
    }

    public class LeaveEmployeeDto
    {
        public int EmployeeID { get; set; }
        //public string RoleName { get; set; }
    }
}
