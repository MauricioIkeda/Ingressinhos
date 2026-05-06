using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Application.Utils.Interface;
using Auth.Application.Utils.Services;
using Auth.Domain.Entities;
using Generic.Application.Dtos;
using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Generic.Infrastructure.Interfaces;
using System.Security.Claims;

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

    public OperationResult<AuthenticateToken> Execute(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return OperationResult<AuthenticateToken>.UnprocessableEntity(new MensagemErro("Login", "Informe e-mail e senha."));
        }

        UserAuth user = _repositorySession.GetRepositoryQuery().Query<UserAuth>(x => x.Email == new Email(email)).ToList()
            .FirstOrDefault();

        if (user == null)
        {
            return OperationResult<AuthenticateToken>.NotFound(new MensagemErro("Login", "Nao encontramos uma conta com esse e-mail."));
        }

        if (!PasswordHash.Verify(password, user.PasswordHash))
        {
            return OperationResult<AuthenticateToken>.Unauthorized(new MensagemErro("Login", "E-mail ou senha invalidos."));
        }

        if (!user.Active)
        {
            return OperationResult<AuthenticateToken>.Forbidden(new MensagemErro("Login", "Usuario inativo."));
        }

        return GenerateAndPersistTokens(user);
    }

    public OperationResult<AuthenticateToken> Refresh(string token, string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
        {
            return OperationResult<AuthenticateToken>.UnprocessableEntity(new MensagemErro("RefreshToken", "Informe token e refresh token."));
        }

        var principal = _token.GetPrincipalFromExpiredToken(token);
        var userAuthId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!long.TryParse(userAuthId, out var id))
        {
            return OperationResult<AuthenticateToken>.Unauthorized(new MensagemErro("RefreshToken", "Token invalido."));
        }

        UserAuth user = _repositorySession.GetRepositoryQuery().Query<UserAuth>(x => x.Id == id).ToList().FirstOrDefault();

        if (user == null)
        {
            return OperationResult<AuthenticateToken>.NotFound(new MensagemErro("RefreshToken", "Usuario nao encontrado."));
        }

        if (!user.Active)
        {
            return OperationResult<AuthenticateToken>.Forbidden(new MensagemErro("RefreshToken", "Usuario inativo."));
        }

        if (!user.IsRefreshTokenValid(refreshToken))
        {
            user.ClearRefreshToken();
            _repositorySession.GetRepository().Upsert(user);
            _repositorySession.GetRepository().Flush().GetAwaiter().GetResult();

            return OperationResult<AuthenticateToken>.Unauthorized(new MensagemErro("RefreshToken", "Refresh token invalido ou expirado."));
        }

        return GenerateAndPersistTokens(user);
    }

    private OperationResult<AuthenticateToken> GenerateAndPersistTokens(UserAuth user)
    {
        var token = _token.Generate(user);
        var refreshToken = _token.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken);
        _repositorySession.GetRepository().Upsert(user);
        _repositorySession.GetRepository().Flush().GetAwaiter().GetResult();

        return OperationResult<AuthenticateToken>.Ok(new AuthenticateToken(token, refreshToken));
    }
}
