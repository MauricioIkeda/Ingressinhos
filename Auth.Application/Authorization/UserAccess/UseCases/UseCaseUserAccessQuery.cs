using Auth.Application.Authorization.UserAccess.Dtos;
using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Domain.Entities;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Auth.Application.Authorization.UserAccess.UseCases;

public class UseCaseUserAccessQuery : IUseCaseUserAccessQuery
{
    private readonly IRepositorySession _repositorySession;

    public UseCaseUserAccessQuery(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult<UserAccessDto> Execute(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return OperationResult<UserAccessDto>.UnprocessableEntity(new MensagemErro("Usuario", "Nao foi possivel identificar o usuario."));
        }

        UserAuth userAuth = _repositorySession.GetRepositoryQuery()
            .Query<UserAuth>(u => u.UserId == userId && u.Active).FirstOrDefault();

        if (userAuth == null)
        {
            return OperationResult<UserAccessDto>.Unauthorized(new MensagemErro("Usuario", "Nao encontramos uma conta ativa para este acesso."));
        }

        return OperationResult<UserAccessDto>.Ok(new UserAccessDto
        {
            UserId = userAuth.UserId,
            Active = userAuth.Active,
            Role = userAuth.Role,
            HasActiveRefreshToken = !string.IsNullOrEmpty(userAuth.RefreshToken),
            TokenIssuedAt = userAuth.TokenIssuedAt == DateTime.MinValue ? null : userAuth.TokenIssuedAt,
            CreatedAtAuth = userAuth.CreatedAtAuth
        });
    }
}
