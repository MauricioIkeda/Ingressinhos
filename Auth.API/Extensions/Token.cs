using Auth.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.API.Extensions
{
    public class Token
    {
        private readonly string _secretKey;

        public Token(IConfiguration configuration)
        {
            _secretKey = configuration.GetSection("AppSettings")["SecretKey"];
        }

        public string Generate(UserAuth user)
        {
            JwtSecurityTokenHandler handler = new();

            byte[] key = Encoding.ASCII.GetBytes(_secretKey);

            SigningCredentials credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor tokenDescriptor = new()
            { 
                //Subject = GenerateClaims(user), corrigir função lá 
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(2),
            };

            SecurityToken token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        //Corrigir
        //private static ClaimsIdentity GenerateClaims(UserAuth user)
        //{
        //    ClaimsIdentity ci = new ClaimsIdentity();
        //    ci.Claims(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));)
        //    ci.AddClaim(new Claim(ClaimTypes.Name, user.N));
        //    ci.AddClaim(new Claim(ClaimTypes.Email, user.Contato.Email));
        //    ci.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));
        //    return ci;
        //}  
        
    }
}
