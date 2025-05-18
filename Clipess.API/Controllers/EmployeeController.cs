using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Newtonsoft.Json;
using Clipess.API.Properties.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Clipess.API.Services;
using System.Net;

namespace Clipess.API.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly EmailService _emailService;
        private static ILog _logger;
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;

        public EmployeeController(IEmployeeRepository employeeRepository, IConfiguration configuration, EmailService emailService)
        {
            _employeeRepository = employeeRepository;
            _emailService = emailService;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _configuration = configuration;
            _cloudinary = new Cloudinary(new Account
            {
                Cloud = _configuration["Cloudinary:CloudName"],
                ApiKey = _configuration["Cloudinary:ApiKey"],
                ApiSecret = _configuration["Cloudinary:ApiSecret"]
            });
        }

        [HttpGet]
        [Route("getEmployee")]
        [HttpGet]
        public async Task<ActionResult> GetEmployees([FromQuery] int EmployeeID)
        {
            try
            {
                var employee = _employeeRepository.GetEmployees();
                if (employee != null)
                {
                    return Ok(employee);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetEmployees)} for EmployeeID: {EmployeeID} exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("getEmployee/{EmployeeID}")]
        public async Task<ActionResult> GetEmployeeById(int EmployeeID)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeById(EmployeeID);
                if (employee == null)
                {
                    //return Ok(employee);
                    return NotFound($"Employee with ID = {EmployeeID} not found");
                }
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetEmployeeById)} for EmployeeID: {EmployeeID} exception: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromForm] EmployeeFormData formData)
        {
            try
            {
                if (formData == null)
                {
                    return BadRequest("Employee details are missing.");
                }

                var otherData = JsonConvert.DeserializeObject<Employee>(formData.OtherData);

                if (otherData == null)
                {
                    return BadRequest("Deserialization of OtherData failed.");
                }

                // Check if the employee already exists based on NIC
                var existingEmployee = await _employeeRepository.GetEmployeeByNIC(otherData.NIC);
                if (existingEmployee != null)
                {
                    if (existingEmployee.Deleted)
                    {
                        return BadRequest("A past employee.");
                    }
                    else
                    {
                        return BadRequest("Employee is available.");
                    }
                }

                var employee = new Employee
                {
                    FirstName = otherData.FirstName,
                    LastName = otherData.LastName,
                    DateOfBirth = otherData.DateOfBirth,
                    NIC = otherData.NIC,
                    PersonalEmail = otherData.PersonalEmail,
                    CompanyEmail = otherData.CompanyEmail,
                    MobileNumber = otherData.MobileNumber,
                    TelephoneNumber = otherData.TelephoneNumber,
                    Address = otherData.Address,
                    Designation = otherData.Designation,
                    ImageUrl = null,
                    FileUrl = null,
                    EmergencyContactPersonName = otherData.EmergencyContactPersonName,
                    EmergencyContactPersonMobileNumber = otherData.EmergencyContactPersonMobileNumber,
                    EmergencyContactPersonAddress = otherData.EmergencyContactPersonAddress,
                    ReportingEmployeeID = otherData.ReportingEmployeeID,
                    EmployeeTypeID = otherData.EmployeeTypeID,
                    RoleID = otherData.RoleID,
                    DepartmentID = otherData.DepartmentID,
                    MaritalStatusID = otherData.MaritalStatusID,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = otherData.CreatedBy,
                    Deleted = false,
                    DeletedDate = otherData.Deleted ? DateTime.Now : (DateTime?)null,
                    DeletedBy = otherData.DeletedBy
                };

                // Upload file
                if (formData.File != null && formData.File.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await formData.File.CopyToAsync(memoryStream);
                        memoryStream.Position = 0; // Reset stream position for upload

                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(formData.File.FileName, memoryStream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        employee.FileUrl = uploadResult.Url.ToString();
                    }
                }

                // Upload image
                if (formData.Image != null && formData.Image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await formData.Image.CopyToAsync(memoryStream);
                        memoryStream.Position = 0; // Reset stream position for upload

                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(formData.Image.FileName, memoryStream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        employee.ImageUrl = uploadResult.Url.ToString();
                    }
                }

                //Generate defaultPassword 
                string defaultPassword = GeneratePassword(8);

                //Hash the password
                employee.Password = BCrypt.Net.BCrypt.HashPassword(defaultPassword);

                _employeeRepository.AddEmployee(employee);
                _employeeRepository.SaveChangesAsync();

                //send email with default password
                string subject = "Your Account Details";
                string body = $"Dear {employee.FirstName},\n\nYour company email address is: {employee.CompanyEmail}\n\nYour default password is: {defaultPassword}\nPlease change your password at: <link to reset page>\n\nRegards,\nCompany Name: Clipess";

                await _emailService.SendEmailAsync(employee.PersonalEmail, subject, body);

                return CreatedAtAction(nameof(AddEmployee), new { employeeId = employee.EmployeeID }, employee);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(AddEmployee)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.ToString() });
            }
        }

        private static string GeneratePassword(int length)
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

        public enum FileType
        {
            Image,
            File
        }


        [HttpPut]
        [Route("UpdateEmployee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromForm] EmployeeFormData formData)
        {
            try
            {
                if (formData == null)
                    return BadRequest("Form data is null");

                var employeeToUpdate = await _employeeRepository.GetEmployee(id);

                if (employeeToUpdate == null)
                    return NotFound($"Employee with Id = {id} not found");

                if (!string.IsNullOrEmpty(formData.OtherData))
                {
                    var otherData = JsonConvert.DeserializeObject<Employee>(formData.OtherData);
                    if (otherData != null)
                    {
                        employeeToUpdate.FirstName = otherData.FirstName;
                        employeeToUpdate.LastName = otherData.LastName;
                        employeeToUpdate.DateOfBirth = otherData.DateOfBirth;
                        employeeToUpdate.NIC = otherData.NIC;
                        employeeToUpdate.PersonalEmail = otherData.PersonalEmail;
                        employeeToUpdate.CompanyEmail = otherData.CompanyEmail;
                        employeeToUpdate.MobileNumber = otherData.MobileNumber;
                        employeeToUpdate.TelephoneNumber = otherData.TelephoneNumber;
                        employeeToUpdate.Address = otherData.Address;
                        employeeToUpdate.Designation = otherData.Designation;
                        employeeToUpdate.ImageUrl = null;
                        employeeToUpdate.FileUrl = null;
                        employeeToUpdate.EmergencyContactPersonName = otherData.EmergencyContactPersonName;
                        employeeToUpdate.EmergencyContactPersonMobileNumber = otherData.EmergencyContactPersonMobileNumber;
                        employeeToUpdate.EmergencyContactPersonAddress = otherData.EmergencyContactPersonAddress;
                        employeeToUpdate.ReportingEmployeeID = otherData.ReportingEmployeeID;
                        employeeToUpdate.EmployeeTypeID = otherData.EmployeeTypeID;
                        employeeToUpdate.RoleID = otherData.RoleID;
                        employeeToUpdate.DepartmentID = otherData.DepartmentID;
                        employeeToUpdate.MaritalStatusID = otherData.MaritalStatusID;
                        employeeToUpdate.CreatedDate = DateTime.Now;
                        employeeToUpdate.CreatedBy = otherData.CreatedBy;
                        employeeToUpdate.Deleted = otherData.Deleted;
                        employeeToUpdate.DeletedDate = otherData.Deleted ? DateTime.Now : (DateTime?)null;
                        employeeToUpdate.DeletedBy = otherData.DeletedBy;
                    }
                }

                if (formData.File != null)
                {
                    if (!IsValidFileType(formData.File, FileType.File))
                    {
                        return BadRequest("Invalid file type. Only pdf files are allowed.");
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        await formData.File.CopyToAsync(memoryStream);
                        memoryStream.Position = 0; // Reset stream position for upload

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(formData.File.FileName, memoryStream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        employeeToUpdate.FileUrl = uploadResult.Url.ToString();
                    }
                }

                if (formData.Image != null)
                {
                    if (!IsValidFileType(formData.Image, FileType.Image))
                    {
                        return BadRequest("Invalid image type. Only jpg, jpeg, png are allowed.");
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await formData.Image.CopyToAsync(memoryStream);
                        memoryStream.Position = 0; // Reset stream position for upload

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(formData.Image.FileName, memoryStream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        employeeToUpdate.ImageUrl = uploadResult.Url.ToString();
                    }
                }

                var updateResult = await _employeeRepository.UpdateEmployee(employeeToUpdate);

                if (updateResult)
                {
                    return Ok(employeeToUpdate);
                }
                else
                {
                    return StatusCode(500, "Failed to update employee");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(UpdateEmployee)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private bool IsValidFileType(IFormFile file, FileType fileType)
        {
            var allowedExtensions = fileType == FileType.File ? new[] { ".pdf" } : new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }

        [HttpPut]
        [Route("UpdateDeleteStatus/{id}")]
        public async Task<IActionResult> UpdateDeleteStatus(int id, [FromBody] Employee request)
        {
            try
            {
                var employeeToDelete = await _employeeRepository.GetEmployeeById(id);

                if (employeeToDelete == null)
                {
                    return NotFound();
                }

                employeeToDelete.Deleted = request.Deleted;
                employeeToDelete.DeletedDate = request.Deleted ? DateTime.Now : (DateTime?)null;
                employeeToDelete.DeletedBy = request.DeletedBy;

                await _employeeRepository.UpdateEmployee(employeeToDelete);
                return Ok(employeeToDelete);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error cccurred in: {nameof(UpdateDeleteStatus)},exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("employeeCounts")]
        public async Task<ActionResult<EmployeeCountsDto>> GetEmployeeCounts()
        {
            try
            {
                var counts = await _employeeRepository.GetEmployeeCountsAsync();
                return Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetEmployeeCounts)}, exception: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("employee-count-by-department")]
        public async Task<ActionResult<DepartmentEmployeeCountDto>> GetEmployeeCountByDepartment()
        {
            try
            {
                var employeeCountByDepartments = await _employeeRepository.GetEmployeeCountByDepartmentAsync();
                return Ok(employeeCountByDepartments);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetEmployeeCounts)}, exception: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        //meka vitharai bhashige api asse mn gahuwe
        [HttpGet]
        [Route("employeeName/{employeeId}")]
        public async Task<IActionResult> GetEmployeeName(int employeeId)
        {
            if (employeeId <= 0)
            {
                return BadRequest("Invalid employee ID.");
            }

            var employeeName = await _employeeRepository.GetEmployeeNameById(employeeId);

            if (string.IsNullOrEmpty(employeeName))
            {
                return NotFound("Employee not found.");
            }

            return Ok(employeeName);
        }
    }
}
