using Auth.Domain.Enums;

namespace Auth.Application.Authorization.UserAccess.Dtos;

public class UserAccessDto
{
    public long UserId { get; set; }
    public bool Active { get; set; }
    public RoleUser Role { get; set; }

    public bool HasActiveRefreshToken { get; set; }   // bool, nunca expor o token
    public DateTime? TokenIssuedAt { get; set; }
    public DateTime? TokenExpiresAt => TokenIssuedAt?.AddHours(2);

    public DateTime CreatedAtAuth { get; set; }

}