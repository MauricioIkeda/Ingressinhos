using Auth.Domain.Entities;
using System.Security.Claims;

namespace Auth.Application.Utils.Interface
{
    public interface IToken
    {
        string Generate(UserAuth user, DateTime expiresAtUtc);
        string GenerateRefreshToken();
        TimeSpan GetAccessTokenLifetime();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
