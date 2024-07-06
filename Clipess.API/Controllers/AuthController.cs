using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Clipess.API.Services;
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Clipess.API.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly EFDbContext _context;
        private readonly AuthService _authService;

        public AuthController(EFDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var employee = _context.Employees.SingleOrDefault(e => e.CompanyEmail == model.CompanyEmail);
            if (employee == null)
                return Unauthorized(new { message = "Invalid email" });

            /*if (!PasswordHasher.VerifyPassword(model.Password, employee.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });*/

            if(employee.Password != model.Password)
                return Unauthorized(new {message = "Invalid credentials"});

            var token = _authService.GenerateJwtToken(employee);

            return Ok(new { token });
        }
    }
}
