using Domain.Entities;
using Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public class JwtProvider : ITokenProvider
    {
        private readonly JwtOptions _jwtOptions;

        public JwtProvider(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<bool> CheckExpireJwtTokenAsync(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);

            return DateTime.UtcNow < token.ValidTo;
        }

        public async Task<string> GenerateTokenAsync(Employee employee)
        {
            var creditians = new SigningCredentials(
                key: new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)
                    ),
                SecurityAlgorithms.HmacSha256
                );
            Claim[] claims = [new("id", employee.Id.ToString())];
            var token = new JwtSecurityToken(
                signingCredentials: creditians,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes)
                );
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenStr;
        }

        public async Task<Guid> GetIdFromJwtTokenAsync(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwtToken);

            var userIdClaim = token.Claims.FirstOrDefault(claim => claim.Type == "id");

            return Guid.Parse(userIdClaim.Value);
        }
    }
}
