using Auth.Application.Authorization.UserAccess.Dtos;
using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Application.Utils.Services;
using Auth.Domain.Entities;
using Auth.Domain.Enums;
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

    public OperationResult<string> Execute(CreateUserAuthDto command)
    {
        if (command is null)
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Body", "Deve ser informado o corpo da requisicao."));
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Name", "Deve ser informado o nome."));
        }

        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Email", "Deve ser informado o email."));
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Password", "Deve ser informada a senha."));
        }

        if (!Enum.IsDefined(typeof(RoleUser), command.Role))
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Role", "Perfil (role) invalido."));
        }

        try
        {
            var email = new Email(command.Email);
            var repositoryQuery = _repositorySession.GetRepositoryQuery();

            var emailInUse = repositoryQuery.Query<UserAuth>(u => u.Email == email).Any();
            if (emailInUse)
            {
                return OperationResult<string>.Fail(new MensagemErro("Email", "Ja existe um usuario cadastrado com esse email."));
            }

            var passwordHash = PasswordHash.Hash(command.Password);
            var user = new UserAuth(command.Name, command.Email, (RoleUser)command.Role, passwordHash);
            if (!user.IsValid)
            {
                return OperationResult<string>.UnprocessableEntity(user.Errors);
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
