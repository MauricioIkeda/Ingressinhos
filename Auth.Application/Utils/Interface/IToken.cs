using Auth.Domain.Entities;
using System.Security.Claims;

namespace Auth.Application.Utils.Interface
{
    public interface IToken
    {
        string Generate(UserAuth user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
