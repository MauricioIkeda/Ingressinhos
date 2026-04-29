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
            return OperationResult<UserAccessDto>.UnprocessableEntity(new MensagemErro("UserId", "Deve ser informado um identificador."));
        }

        UserAuth userAuth = _repositorySession.GetRepositoryQuery()
            .Query<UserAuth>(u => u.UserId == userId && u.Active).FirstOrDefault();

        if (userAuth == null)
        {
            return OperationResult<UserAccessDto>.Unauthorized(new MensagemErro("UserId", "Nenhum usuario ativo encontrado."));
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
