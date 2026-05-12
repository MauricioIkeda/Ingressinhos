using Auth.Application.Authorization.UserAccess.Dtos;
using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Application.Utils.Services;
using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Generic.Infrastructure.Interfaces;

namespace Auth.Application.Authorization.UserAccess.UseCases;

public class CreateAdminUserAuth : IUseCaseCreateAdminUserAuth  // Apenas para criar o admin
{
    private readonly IRepositorySession _repositorySession;

    public CreateAdminUserAuth(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult<string> Execute(CreateAdminRequest command, bool isAuthenticated, bool isAdmin)
    {
        if (command == null)
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Requisicao", "Envie os dados do administrador."));
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Senha", "Informe uma senha."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var hasAnyAdmin = repositoryQuery.Query<UserAuth>(u => u.Role == RoleUser.Admin).Any();  // Se não tiver admin cria, se tiver exige autenticação, forma bucha, mas vai ser assim

            if (hasAnyAdmin)
            {
                if (!isAuthenticated)
                {
                    return OperationResult<string>.Unauthorized(new MensagemErro("Administrador", "Autentique-se para criar um novo administrador."));
                }

                if (!isAdmin)
                {
                    return OperationResult<string>.Forbidden(new MensagemErro("Administrador", "Somente administradores podem criar novos administradores."));
                }
            }

            var email = new Email(command.Email);
            if (!email.IsValid)
            {
                return email.ToUnprocessableEntityResult<string>();
            }

            var emailInUse = repositoryQuery.Query<UserAuth>(u => u.Email == email).Any();
            if (emailInUse)
            {
                return OperationResult<string>.Fail(new MensagemErro("E-mail", "Ja existe uma conta cadastrada com esse e-mail."));
            }

            var passwordHash = PasswordHash.Hash(command.Password);
            var user = new UserAuth(command.Name, command.Email, RoleUser.Admin, passwordHash);
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
