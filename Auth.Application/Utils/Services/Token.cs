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
        private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromHours(2);
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

        public string Generate(UserAuth user, DateTime expiresAtUtc)
        {
            JwtSecurityTokenHandler handler = new();

            byte[] key = Encoding.ASCII.GetBytes(_secretKey); // Converter a chave secreta para bytes, formato esperado pelo algoritmo de assinatura

            SigningCredentials credentials = new SigningCredentials( new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature); // Assinar o token com HMAC-SHA256 para a API conseguir validar que ele nao foi adulterado

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = GenerateClaims(user), // Claims que vão no payload do JWT e que o front consegue ler sem precisar da chave
                SigningCredentials = credentials,
                Expires = expiresAtUtc,
            };

            if (!string.IsNullOrWhiteSpace(_issuer))
            {
                tokenDescriptor.Issuer = _issuer;
            }

            if (!string.IsNullOrWhiteSpace(_audience))
            {
                tokenDescriptor.Audience = _audience;
            }

            SecurityToken token = handler.CreateToken(tokenDescriptor); // Montar o JWT com header, payload e assinatura

            return handler.WriteToken(token);
        }

        public TimeSpan GetAccessTokenLifetime()
        {
            return AccessTokenLifetime; // Fiz essa merda ai para ser sempre o tempo de vida
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64]; // Token aleatorio puro, sem payload legivel, usado so para renovação
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // Ainda valida a assinatura, então token fake continua sendo rejeitado
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey)),
                ValidateIssuer = !string.IsNullOrWhiteSpace(_issuer),
                ValidIssuer = _issuer,
                ValidateAudience = !string.IsNullOrWhiteSpace(_audience),
                ValidAudience = _audience,
                ValidateLifetime = false, // No refresh precisamos ler um access token já expirado para descobrir de quem ele era
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(token, tokenValidationParameters, out var securityToken); // Validar assinatura/issuer/audience e extrair os claims

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
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
