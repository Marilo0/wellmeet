using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Wellmeet.Core.Enums;
using Wellmeet.Services.Interfaces;

namespace Wellmeet.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(int userId, string username, string email, UserRole role)
        {
            var keyString = _configuration["Jwt:SecretKey"]
                ?? throw new Exception("JWT SecurityKey missing in configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claimsInfo = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],  //"https://localhost:5001"
                audience: _configuration["Jwt:Audience"],  //"https://localhost:5001"
                claims: claimsInfo,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
