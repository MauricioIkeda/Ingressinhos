using Auth.Domain.Enums;

namespace Auth.Application.Authorization.UserAccess.Dtos;

public class UserAccessDto
{
    public string UserId { get; set; }
    public bool Active { get; set; }
    public RoleUser Role { get; set; }

    public bool HasActiveRefreshToken { get; set; }   // bool, nunca expor o token
    public DateTime? TokenIssuedAt { get; set; }
    public DateTime? TokenExpiresAt => TokenIssuedAt?.AddDays(1);

    public DateTime CreatedAtAuth { get; set; }

}