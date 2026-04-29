using Auth.Application.Authorization.UserAccess.Dtos;
using Generic.Domain.Entities;

namespace Auth.Application.Authorization.UserAccess.Interfaces;

public interface IUseCaseCreateUserAuth
{
    OperationResult<string> Execute(CreateUserAuthDto command);
}
