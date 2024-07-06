using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clipess.DBClient.EntityModels;
using Microsoft.Extensions.Options;
using System.Linq;
using Clipess.DBClient.Repositories;

namespace Clipess.API.Services
{
    public class AuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly EFDbContext _context;

        public AuthService(IOptions<JwtSettings> jwtSettings,EFDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        public string GenerateJwtToken(Employee employee)
        {
            var roleName = _context.Roles
                .Where(r => r.RoleID  == employee.RoleID)
                .Select(r => r.RoleName)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(roleName))
            {
                throw new Exception("Role not found for the given RoleID");
            }
            //Normalize roleName to lower case
            var normalizedRoleName = roleName.ToLower();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.CompanyEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("EmployeeID",employee.EmployeeID.ToString()),
                new Claim("RoleID",employee.RoleID.ToString()),
                new Claim(ClaimTypes.Role,normalizedRoleName)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
