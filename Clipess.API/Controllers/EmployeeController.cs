using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using BCrypt.Net;
using Clipess.API.Services;

namespace Clipess.API.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly EmailService _emailService;
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(EmployeeController));

        public EmployeeController(IEmployeeRepository employeeRepository,EmailService emailService)
        {
            _employeeRepository = employeeRepository;
            _emailService = emailService;
        }

        [Route("AddEmployee")]
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDto employeeDto)
        { 
            if (employeeDto == null)
            {
                return BadRequest(new { message = "Invalid employee data." });
            }

            try
            {
                Employee newEmployee = new Employee()
                {
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    DateOfBirth = employeeDto.DateOfBirth,
                    NIC = employeeDto.NIC,
                    PersonalEmail = employeeDto.PersonalEmail,
                    CompanyEmail = employeeDto.CompanyEmail,
                    MobileNumber = employeeDto.MobileNumber,
                    TelephoneNumber = employeeDto.TelephoneNumber,
                    Address = employeeDto.Address,
                    Designation = employeeDto.Designation,
                    ImageUrl = employeeDto.ImageUrl,
                    FileUrl = employeeDto.FileUrl,
                    EmergencyContactPersonName = employeeDto.EmergencyContactPersonName,
                    EmergencyContactPersonMobileNumber = employeeDto.EmergencyContactPersonMobileNumber,
                    EmergencyContactPersonAddress = employeeDto.EmergencyContactPersonAddress,
                    ReportingEmployeeID = employeeDto.ReportingEmployeeID,
                    EmployeeTypeID = employeeDto.EmployeeTypeID,
                    RoleID = employeeDto.RoleID,
                    DepartmentID = employeeDto.DepartmentID,
                    MaritalStatusID = employeeDto.MaritalStatusID,
                    CreatedDate = DateTime.Now,
                    CreatedBy = 1, // Assuming '1' is the admin ID, replace with actual admin ID
                    Deleted = false
                };
                //Generate defaultPassword 
                string defaultPassword = GeneratePassword(8);

                //Hash the password
                newEmployee.Password = BCrypt.Net.BCrypt.HashPassword(defaultPassword);

                var addedEmployee = await _employeeRepository.AddEmployeeAsync(newEmployee);

                //send email with default password
                string subject = "Your Account Details";
                string body = $"Dear {employeeDto.FirstName},\n\nYour default password is: {defaultPassword}\nPlease change your password at: <link to reset page>\n\nRegards,\nCompany Name: Clipess";

                await _emailService.SendEmailAsync(newEmployee.PersonalEmail, subject, body);

                return Ok("Employee added successfully!");
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(AddEmployee)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        private string GeneratePassword(int length)
        {
             const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
             var random = new Random();
             var password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }
            return new string(password);

        }
            
    }
}
