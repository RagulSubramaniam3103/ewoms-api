using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ExternalClassLibrary_DTO.UserData_DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.JWTToken
{
    public class LoginAuthentication_Token
    {
        private readonly IConfiguration _configuration;
        public LoginAuthentication_Token(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> GenerateToken(MasterUser_TokenDTO masterUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, masterUser.UserId),
                new Claim(ClaimTypes.Name, masterUser.UserName),
                new Claim(ClaimTypes.Email, masterUser.Email),
                new Claim(ClaimTypes.Role, masterUser.Roles)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var crediential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));

            var tokengenerated = new JwtSecurityToken(
                issuer : _configuration["Jwt:Issuer"],
                audience : _configuration["Jwt:Audience"],
                claims : claims,
                expires : expiry,
                signingCredentials : crediential
                );
            return new JwtSecurityTokenHandler().WriteToken(tokengenerated);
        }
    }
}
