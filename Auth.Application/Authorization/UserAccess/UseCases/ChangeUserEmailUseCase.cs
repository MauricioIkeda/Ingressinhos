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
        if (command is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Body", "Deve ser informado o corpo da requisicao."));
        }

        if (string.IsNullOrWhiteSpace(command.UserId))
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("UserId", "Deve ser informado o identificador do usuario."));
        }

        if (string.IsNullOrWhiteSpace(command.NewEmail))
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("NewEmail", "Deve ser informado o novo email."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var user = repositoryQuery.Query<UserAuth>(u => u.UserId == command.UserId && u.Active).FirstOrDefault();
            if (user is null)
            {
                return OperationResult.NotFound(new MensagemErro("UserId", "Usuario nao encontrado."));
            }

            user.ChangeEmail(command.NewEmail);
            if (!user.IsValid)
            {
                return user.ToUnprocessableEntityResult();
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
