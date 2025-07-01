using Messenger.Domain;
using Messenger.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Messenger.App.Authentication
{
    public class JwtTokenGenerator : IJwtTokenGenerator 
    {
        private readonly JwtOptions _options;
        public JwtTokenGenerator(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }
        public string GenerateToken(User user)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("PhoneNumber", user.PhoneNuber ?? string.Empty),
            };
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
                SecurityAlgorithms.HmacSha256
            );
            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddHours(_options.ExpirationTime)
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
