using Auth.Application.Authorization.UserAccess.Dtos;
using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Domain.Entities;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Auth.Application.Authorization.UserAccess.UseCases;

public class ChangeUserEmailUseCase : IUseCaseChangeUserEmail
{
    private readonly IRepositorySession _repositorySession;

    public ChangeUserEmailUseCase(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(ChangeUserEmailDto command)
    {
        if (command == null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Requisicao", "Envie os dados da alteracao."));
        }

        if (string.IsNullOrWhiteSpace(command.UserId))
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Usuario", "Nao foi possivel identificar o usuario."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var user = repositoryQuery.Query<UserAuth>(u => u.UserId == command.UserId && u.Active).FirstOrDefault();
            if (user is null)
            {
                return OperationResult.NotFound(new MensagemErro("Usuario", "Nao encontramos a conta informada."));
            }

            user.ChangeEmail(command.NewEmail);
            if (!user.IsValid)
            {
                return OperationResult.UnprocessableEntity(user.Errors);
            }

            var repository = _repositorySession.GetRepository();
            repository.Upsert(user);
            repository.Flush().GetAwaiter().GetResult();

            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
