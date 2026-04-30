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
            AddError("Nome", "Informe o nome.");
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
            AddError("Perfil", "Informe um perfil de acesso valido.");
        }
        else
        {
            Role = role;
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            AddError("Senha", "Nao foi possivel processar a senha informada.");
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
            AddError("Senha", "Nao foi possivel processar a nova senha.");
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
