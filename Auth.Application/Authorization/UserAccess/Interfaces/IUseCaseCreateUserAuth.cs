using Generic.Application.Dtos;
using Generic.Domain.Entities;

namespace Auth.Application.Authorization.UserAccess.Interfaces;

public interface IUseCaseCreateUserAuth
{
    OperationResult<string> Execute(CreateUserRequest command);
}
