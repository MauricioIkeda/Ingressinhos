using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Auth.Entities;

public class UserRole : BaseEntity
{
    public Guid UserAuthId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public UserRole(Guid userAuthId, Guid roleId)
    {
        if (userAuthId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o usuario de autenticacao");
        }

        if (roleId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o perfil");
        }

        Id = Guid.NewGuid();
        UserAuthId = userAuthId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
    }
}