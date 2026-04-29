using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Application.Utils.Interface;
using Auth.Application.Utils.Services;
using Auth.Domain.Entities;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

public class AuthenticateUserUseCase : IUseCaseUserAuthCollection
{
    private readonly IRepositoryQuery _repositoryQuery;
    private readonly IToken _token;

    public AuthenticateUserUseCase(IRepositoryQuery repositoryQuery, IToken token)
    {
        _repositoryQuery = repositoryQuery;
        _token = token;
    }

    public OperationResult<string> Execute(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return OperationResult<string>.UnprocessableEntity(new MensagemErro("Login", "Email e senha sao obrigatorios."));
        }

        var user = _repositoryQuery.Query<UserAuth>(x => x.Email.Endereco == email)
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

        return OperationResult<string>.Ok(token);
    }
}
