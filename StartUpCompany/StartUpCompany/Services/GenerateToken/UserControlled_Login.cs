using Microsoft.IdentityModel.Tokens;
using StartUpCompany.CQRSMethod.Queries.UserControlled;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StartUpCompany.Services.GenerateToken
{
    public class UserControlled_Login
    {
        private readonly IConfiguration _configuration;
        public UserControlled_Login(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> GenerateToken(LoginCommandResponse resultlogin)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, resultlogin.UserLogin),
                new Claim(ClaimTypes.Role, resultlogin.UserRole)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: creds
                );

            var finaltokengeneration = new JwtSecurityTokenHandler().WriteToken(token);

            return finaltokengeneration;
        }
    }
}
