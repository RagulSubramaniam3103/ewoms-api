using IdentityWebAPI_User.MainModel;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityWebAPI_User.Data
{
    public class GenerateToken
    {
        private readonly IConfiguration _configuration;
        public GenerateToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string TokenGenerate(CustomerDetails customerdetails, IList<string> Roles)
        {
            var privatekey = _configuration["Jwt:Key"];
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customerdetails.Id.ToString()),
                new Claim(ClaimTypes.Name, customerdetails.UserName),
                new Claim(ClaimTypes.Email, customerdetails.Email)
            };
            foreach(var roles in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, roles));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privatekey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "Test",
                audience: "Test",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
                );
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
