using Generic.Application.Dtos;
using Generic.Domain.Entities;

namespace Auth.Application.Authorization.UserAccess.Interfaces;

public interface IUseCaseUserAuthCollection
{
    OperationResult<AuthenticateResponse> Execute(string email, string password);
    OperationResult<AuthenticateResponse> Refresh(string token, string refreshToken);
}
