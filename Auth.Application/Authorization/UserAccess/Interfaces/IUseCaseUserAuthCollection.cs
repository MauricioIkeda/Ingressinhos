using Generic.Application.Dtos;
using Generic.Domain.Entities;

namespace Auth.Application.Authorization.UserAccess.Interfaces;

public interface IUseCaseUserAuthCollection
{
    OperationResult<AuthenticateToken> Execute(string email, string password);
    OperationResult<AuthenticateToken> Refresh(string token, string refreshToken);
}
