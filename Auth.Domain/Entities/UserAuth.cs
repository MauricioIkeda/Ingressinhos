using Auth.Domain.Enums;
using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;

namespace Auth.Domain.Entities;

public class UserAuth : BaseEntity
{
    public string UserId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = new(string.Empty);
    public string PasswordHash { get; private set; } = string.Empty;
    public bool Active { get; private set; } = true;
    public RoleUser Role { get; private set; }
    public DateTime CreatedAtAuth { get; private set; }
    public DateTime? LastPasswordChangedAt { get; private set; }
    public string RefreshToken { get; private set; } = string.Empty;
    public DateTime? RefreshTokenExpiration { get; private set; }
    public DateTime TokenIssuedAt { get; private set; }

    protected UserAuth()
    {
    }

    public UserAuth(string name, string email, RoleUser role, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            AddError("Name", "Deve ser informado o nome do usuario para autenticacao");
        }
        else
        {
            Name = name.Trim();
        }

        Email emailValue = new Email(email);
        CopyErrorsFrom(emailValue);
        if (emailValue.IsValid)
        {
            Email = emailValue;
        }

        if (role <= 0)
        {
            AddError("Role", "Deve ser informado o perfil do usuario");
        }
        else
        {
            Role = role;
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            AddError("PasswordHash", "Deve ser informado o hash da senha");
        }
        else
        {
            PasswordHash = passwordHash;
        }

        UserId = Guid.NewGuid().ToString();
        CreatedAtAuth = DateTime.UtcNow;
    }

    public void ChangeEmail(string email)
    {
        ClearErrors();

        Email emailValue = new Email(email);
        CopyErrorsFrom(emailValue);
        if (!emailValue.IsValid)
        {
            return;
        }

        Email = emailValue;
    }

    public void ChangePassword(string newPasswordHash)
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            AddError("PasswordHash", "Deve ser informado o hash da nova senha");
            return;
        }

        PasswordHash = newPasswordHash.Trim();
        LastPasswordChangedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        ClearErrors();
        Active = false;
    }

    public void Activate()
    {
        ClearErrors();
        Active = true;
    }

    public void SetRefreshToken(string refreshToken)
    {
        ClearErrors();
        RefreshToken = refreshToken;
        TokenIssuedAt = DateTime.UtcNow;
        RefreshTokenExpiration = DateTime.UtcNow.AddDays(1);
    }

    public void ClearRefreshToken()
    {
        ClearErrors();
        RefreshToken = string.Empty;
        TokenIssuedAt = DateTime.MinValue;
        RefreshTokenExpiration = null;
    }

    public bool IsRefreshTokenValid(string refreshToken)
    {
        return DateTime.UtcNow < RefreshTokenExpiration;
    }
}
