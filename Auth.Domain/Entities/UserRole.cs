using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Auth.Entities;

public class UserRole : BaseEntity
{
    public long UserAuthId { get; private set; }
    public long RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public UserRole(long userAuthId, long roleId)
    {
        if (userAuthId <= 0)
        {
            throw new Exception("Deve ser informado o usuario de autenticacao");
        }

        if (roleId <= 0)
        {
            throw new Exception("Deve ser informado o perfil");
        }

        UserAuthId = userAuthId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
    }
}