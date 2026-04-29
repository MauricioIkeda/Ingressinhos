using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Application.Utils.Interface;
using Auth.Application.Utils.Services;
using Auth.Domain.Entities;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

public class AuthenticateUserUseCase : IUseCaseUserAuthCollection
{
    public ListMessages Messages { get; }

    private readonly IRepositoryQuery _repositoryQuery;
    private readonly IToken _token;

    public AuthenticateUserUseCase(IRepositoryQuery repositoryQuery, IToken token)
    {
        _repositoryQuery = repositoryQuery;
        _token = token;
        Messages = new ListMessages();
    }

    public (bool success, string token) Execute(string email, string password)
    {
        Messages.Clear();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            Messages.Add("Email e senha são obrigatórios.");
            return (false, null);
        }

        var user = _repositoryQuery.Query<UserAuth>(x => x.Email.Endereco == email)
            .FirstOrDefault();

        if (user == null)
        {
            Messages.Add("Nenhum usuário encontrado para esse email.");
            return (false, null);
        }

        if (!PasswordHash.Verify(password, user.PasswordHash))
        {
            Messages.Add("Senha errada.");
            return (false, null);
        }

        var token = _token.Generate(user);

        return (true, token);
    }
}