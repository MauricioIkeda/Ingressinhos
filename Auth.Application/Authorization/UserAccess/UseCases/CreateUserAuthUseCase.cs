using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Application.Utils.Services;
using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Generic.Application.Dtos;
using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Generic.Infrastructure.Interfaces;

namespace Auth.Application.Authorization.UserAccess.UseCases;

public class CreateUserAuthUseCase : IUseCaseCreateUserAuth
{
    private readonly IRepositorySession _repositorySession;

    public CreateUserAuthUseCase(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult<string> Execute(CreateUserRequest command)
    {
        if (command == null)
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Requisicao", "Envie os dados do usuario."));
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Senha", "Informe uma senha."));
        }

        try
        {
            Email email = new Email(command.Email);
            if (!email.IsValid)
            {
                return email.ToUnprocessableEntityResult<string>();
            }

            var repositoryQuery = _repositorySession.GetRepositoryQuery();

            var emailInUse = repositoryQuery.Query<UserAuth>(u => u.Email == email).Any();
            if (emailInUse)
            {
                return OperationResult<string>.Fail(new MensagemErro("E-mail", "Ja existe uma conta cadastrada com esse e-mail."));
            }

            var passwordHash = PasswordHash.Hash(command.Password);
            var user = new UserAuth(command.Name, command.Email, (RoleUser)command.Role, passwordHash);
            if (!user.IsValid)
            {
                return user.ToUnprocessableEntityResult<string>();
            }

            var repository = _repositorySession.GetRepository();
            repository.Include(user);
            repository.Flush().GetAwaiter().GetResult();

            return OperationResult<string>.Created(user.UserId);
        }
        catch (Exception ex)
        {
            return OperationResult<string>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
