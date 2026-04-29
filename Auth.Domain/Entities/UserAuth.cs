using Auth.Domain.Enums;
using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;

namespace Auth.Domain.Entities;

public class UserAuth : BaseEntity
{
    public string UserId { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool Active { get; private set; } = true;
    public RoleUser Role { get; private set; }
    public DateTime CreatedAtAuth { get; private set; }
    public DateTime? LastPasswordChangedAt { get; private set; }
    public string RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiration { get; private set; }
    public DateTime TokenIssuedAt { get; private set; }

    protected UserAuth()
    {
    }

    public UserAuth(string name, string email, RoleUser role, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Deve ser informado o nome do usuario para autenticacao");
        }

        if (email == null)
        {
            throw new Exception("Deve ser informado o email do usuario para autenticacao");
        }

        if (role <= 0)
        {
            throw new Exception("Deve ser informado o perfil do usuario");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new Exception("Deve ser informado o hash da senha");
        }
        UserId = Guid.NewGuid().ToString();
        Name = name.Trim();
        Email = new Email(email);
        Role = role;
        PasswordHash = passwordHash;
        CreatedAtAuth = DateTime.UtcNow;
    }

    public void ChangeEmail(string email)
    {
        Email = new Email(email);
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

    public void SetRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
        TokenIssuedAt = DateTime.UtcNow;
        RefreshTokenExpiration = DateTime.UtcNow.AddDays(1);
    }

    public void ClearRefreshToken()
    {
        RefreshToken = string.Empty;
        TokenIssuedAt = DateTime.MinValue;
        RefreshTokenExpiration = null;
    }

    public bool IsRefreshTokenValid(string refreshToken)
    {
        return DateTime.UtcNow < RefreshTokenExpiration;
    }

    public bool VerifyPassword(string password)
    {
        return PasswordHash.Verify(password, PasswordHash);
    }
}