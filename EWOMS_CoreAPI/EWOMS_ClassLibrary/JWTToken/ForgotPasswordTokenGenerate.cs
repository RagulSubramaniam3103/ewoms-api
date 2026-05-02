using EWOMS_ExternalClassLibrary_DTO.UserData_DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
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
    public class ForgotPasswordTokenGenerate
    {
        private readonly IConfiguration _configuration;
        public ForgotPasswordTokenGenerate(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> HandleToken(Master_EmailSent user_TokenDTO)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user_TokenDTO.Email)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));


            var credentital = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var Expirydata = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ForgotExpiryMinutes"]));

            var tokengenerate = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: Expirydata,
                signingCredentials: credentital
                );

            var tokenHandler = new JwtSecurityTokenHandler();
            var finalToken = tokenHandler.WriteToken(tokengenerate);

            return finalToken.ToString();
        }
    }
}
