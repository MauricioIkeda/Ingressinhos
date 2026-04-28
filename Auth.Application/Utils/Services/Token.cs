using Auth.Application.Utils.Interface;
using Auth.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Application.Utils.Services
{
    public class Token : IToken
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        public Token(string secretKey, string issuer, string audience)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException(
                    "AppSettings:SecretKey não foi configurada. Defina no appsettings.json/appsettings.Development.json ou via variável de ambiente AppSettings__SecretKey.");
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;

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
                Subject = GenerateClaims(user),
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(2),
            };
            if (!string.IsNullOrWhiteSpace(_issuer))
                tokenDescriptor.Issuer = _issuer;

            if (!string.IsNullOrWhiteSpace(_audience))
                tokenDescriptor.Audience = _audience;

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
        private static ClaimsIdentity GenerateClaims(UserAuth user)
        {
            ClaimsIdentity ci = new ClaimsIdentity();
            ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            ci.AddClaim(new Claim(ClaimTypes.UserData, user.UserId.ToString()));
            ci.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            ci.AddClaim(new Claim(ClaimTypes.Email, user.Email.Endereco));
            ci.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));
            return ci;
        }
    }
}
