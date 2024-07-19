using Microsoft.AspNetCore.Mvc;
using Clipess.API.Services;
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Repositories;
using Clipess.DBClient.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Clipess.API.Properties.Services;

namespace Clipess.API.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly EFDbContext _context;
        private readonly AuthService _authService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<AuthController> _logger;
        private readonly EmailService _emailService;

        public AuthController(EFDbContext context, AuthService authService, IEmployeeRepository employeeRepository, ILogger<AuthController> logger, EmailService emailService)
        {
            _context = context;
            _authService = authService;
            _employeeRepository = employeeRepository;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.CompanyEmail) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Invalid login request.");
            }

            // Retrieve the employee record based on the email
            var employee = await _employeeRepository.GetEmployeeByEmail(model.CompanyEmail);

            if (employee == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            // Verify the password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, employee.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = _authService.GenerateJwtToken(employee);

            Response.Cookies.Append("jwtToken", token);

            return Ok(new { token });
        }

        [HttpPost]
        [Route("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChange model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.CompanyEmail) || string.IsNullOrWhiteSpace(model.CurrentPassword) || string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return BadRequest("Invalid password change request.");
            }

            if (string.IsNullOrEmpty(model.CompanyEmail))
            {
                return Unauthorized("Invalid credentials.");
            }

            var employee = await _employeeRepository.GetEmployeeByEmail(model.CompanyEmail);

            if (employee == null)
            {
                _logger.LogWarning("No employee found with email {Email}", model.CompanyEmail);
                return Unauthorized("Invalid credentials.");
            }

            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(model.CurrentPassword, employee.Password);

            if (!isCurrentPasswordValid)
            {
                _logger.LogWarning("Current password is invalid for employee with email {Email}", model.CompanyEmail);
                return Unauthorized("Invalid current password.");
            }

            var result = await _authService.PasswordChange(employee, model.NewPassword);

            if (!result)
            {
                return StatusCode(500, "An error occurred while changing the password.");
            }

            return Ok("Password changed successfully.");
        }

        [HttpPost]
        [Route("forgotPassword")]

        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel forgotPassword)
        {
            var user = await _employeeRepository.GetEmployeeByEmail(forgotPassword.CompanyEmail);

            if (user == null)
            {
                return BadRequest("user does not exist with this email");
            }
            var token = _authService.GenerateJwtToken(user);
            string subject = "Change your password using this link:";
            var resetLink = $"http://localhost:3000/reset-password?email={user.CompanyEmail}&token={WebUtility.UrlEncode(token)}";

            await _emailService.SendEmailAsync(user.CompanyEmail, subject, resetLink);

            return Ok("Reset link has been sent to your email.");

        }

        [HttpPost]
        [Route("resetPassword")]

        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordModel resetPassword)
        {

            if (resetPassword.NewPassword != resetPassword.ConfirmPassword)
            {
                return BadRequest("Password mismatch");
            }
            var user = await _employeeRepository.GetEmployeeByEmail(resetPassword.CompanyEmail);
            if (user == null)
            {
                return BadRequest("user does not exist with this email");
            }

            var result = await _employeeRepository.ResetPassword(user, BCrypt.Net.BCrypt.HashPassword(resetPassword.NewPassword));


            if (!result)
            {
                return StatusCode(500, "An error occurred while changing the password.");
            }

            return Ok("Password changed successfully.");


        }

    }
}
