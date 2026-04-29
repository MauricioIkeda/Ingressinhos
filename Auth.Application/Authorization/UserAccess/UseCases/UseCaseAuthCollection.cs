using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Application.Utils.Interface;
using Auth.Application.Utils.Services;
using Auth.Domain.Entities;
using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Generic.Infrastructure.Interfaces;

namespace Auth.Application.Authorization.UserAccess.UseCases;

public class AuthenticateUserUseCase : IUseCaseUserAuthCollection
{
    private readonly IRepositorySession _repositorySession;
    private readonly IToken _token;

    public AuthenticateUserUseCase(IRepositorySession repositorySession, IToken token)
    {
        _repositorySession = repositorySession;
        _token = token;
    }

    public OperationResult<string> Execute(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Login", "Email e senha sao obrigatorios."));
        }

        UserAuth user = _repositorySession.GetRepositoryQuery().Query<UserAuth>(x => x.Email == new Email(email)).ToList()
            .FirstOrDefault();

        if (user == null)
        {
            return OperationResult<string>.NotFound(new MensagemErro("Email", "Nenhum usuario encontrado para esse email."));
        }

        if (!PasswordHash.Verify(password, user.PasswordHash))
        {
            return OperationResult<string>.Unauthorized(new MensagemErro("Password", "Senha errada."));
        }

        var token = _token.Generate(user);
        user.SetRefreshToken(token);

        _repositorySession.GetRepository().Upsert(user);
        _repositorySession.GetRepository().Flush().GetAwaiter();

        return OperationResult<string>.Ok(token);
    }
}
