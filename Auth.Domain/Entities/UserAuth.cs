using Generic.Domain.Entities;
using Auth.Domain.Enums;

namespace Ingressinhos.Domain.Auth.Entities;

public class UserAuth : BaseEntity
{
    public Guid UserId { get; private set; }
    public string PasswordHash { get; private set; }
    public AuthUserStatus Status { get; private set; }
    public bool IsActive => Status == AuthUserStatus.Active;
    public DateTime CreatedAtAuth { get; private set; }
    public DateTime? LastPasswordChangedAt { get; private set; }

    public UserAuth(Guid userId, string passwordHash)
    {
        if (userId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o usuario para autenticacao");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new Exception("Deve ser informado o hash da senha");
        }

        Id = Guid.NewGuid();
        UserId = userId;
        PasswordHash = passwordHash.Trim();
        Status = AuthUserStatus.Active;
        CreatedAtAuth = DateTime.UtcNow;
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
        Status = AuthUserStatus.Inactive;
    }

    public void Activate()
    {
        Status = AuthUserStatus.Active;
    }
}