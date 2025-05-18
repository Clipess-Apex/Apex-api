using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Clipess.DBClient.Repositories;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Clipess.API.Services
{
    public class AuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly EFDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IOptions<JwtSettings> jwtSettings, EFDbContext context, ILogger<AuthService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
            _logger = logger;
        }

        public string GenerateJwtToken(Employee employee)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var roleName = _context.Roles
                .Where(r => r.RoleID == employee.RoleID)
                .Select(r => r.RoleName)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(roleName))
            {
                throw new Exception("Role not found for the given RoleID");
            }
            //Normalize roleName to lower case
            var normalizedRoleName = roleName.ToLower();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.CompanyEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("EmployeeID",employee.EmployeeID.ToString()),
                new Claim("RoleID",employee.RoleID.ToString()),
                new Claim("ImageUrl",employee.ImageUrl?? string.Empty),
                new Claim("FirstName",employee.FirstName?? string.Empty),
                new Claim("LastName",employee.LastName?? string.Empty),
                new Claim(ClaimTypes.Role,normalizedRoleName)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes),
                signingCredentials: credentials);

            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> PasswordChange(Employee employee, string newPassword)
        {
            try
            {
                employee.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing the password for employee with email {Email}", employee.CompanyEmail);
                return false;
            }
        }
    }
}
