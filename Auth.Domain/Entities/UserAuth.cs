using Auth.Domain.Enums;
using Generic.Domain.Entities;

namespace Auth.Domain.Entities;

public class UserAuth : BaseEntity
{
    public long UserId { get; private set; }
    public string PasswordHash { get; private set; }
    public bool Active { get; private set; } = true;
    public RoleUser Role { get; private set; } = new RoleUser();
    public DateTime CreatedAtAuth { get; private set; }
    public DateTime? LastPasswordChangedAt { get; private set; }
    public string RefreshToken { get; private set; }
    public DateTime TokenIssuedAt { get; private set; }

    protected UserAuth()
    {
    }

    public UserAuth(long userId, string passwordHash)
    {
        ValidateUser(userId);

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new Exception("Deve ser informado o hash da senha");
        }

        UserId = userId;
        PasswordHash = passwordHash;
        CreatedAtAuth = DateTime.UtcNow;
    }

    public void ChangeUser(long userId)
    {
        ValidateUser(userId);
        UserId = userId;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            throw new Exception("Deve ser informado o hash da nova senha");
        }

        PasswordHash = newPasswordHash.Trim();
        LastPasswordChangedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Active = false;
    }

    public void Activate()
    {
        Active = true;
    }

    private static void ValidateUser(long userId)
    {
        if (userId <= 0)
        {
            throw new Exception("Deve ser informado o identificador do usuario para autenticacao");
        }
    }

    public void SetRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
        TokenIssuedAt = DateTime.UtcNow;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = string.Empty;
        TokenIssuedAt = DateTime.MinValue;
    }

    public bool IsRefreshTokenValid(string refreshToken)
    {
        if (string.IsNullOrEmpty(RefreshToken)) return false;
        if (RefreshToken != refreshToken) return false;

        var expirationDate = TokenIssuedAt.AddDays(7);
        return DateTime.UtcNow < expirationDate;
    }
}