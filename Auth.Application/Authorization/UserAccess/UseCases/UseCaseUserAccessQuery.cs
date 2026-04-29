using Auth.Application.Authorization.UserAccess.Dtos;
using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Domain.Entities;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Auth.Application.Authorization.UserAccess.UseCases;

public class UseCaseUserAccessQuery : IUseCaseUserAccessQuery
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public UseCaseUserAccessQuery(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public UserAccessDto Execute(string userId)
    {
        Messages.Clear();

        if (string.IsNullOrEmpty(userId))
        {
            Messages.Add("Deve ser informado um identificador", error: true);
            return null;
        }

        UserAuth userAuth = _repositorySession.GetRepositoryQuery()
            .Query<UserAuth>(u => u.UserId == userId && u.Active).FirstOrDefault();

        if (userAuth == null)
        {
            Messages.Add("Nenhum usuario ativo encontrado", error: true);
            return null;
        }

        return new UserAccessDto
        {
            UserId = userAuth.UserId,
            Active = userAuth.Active,
            Role = userAuth.Role,
            HasActiveRefreshToken = !string.IsNullOrEmpty(userAuth.RefreshToken),
            TokenIssuedAt = userAuth.TokenIssuedAt == DateTime.MinValue ? null : userAuth.TokenIssuedAt,
            CreatedAtAuth = userAuth.CreatedAtAuth
        };
    }
}
