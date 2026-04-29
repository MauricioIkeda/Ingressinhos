using Auth.Application.Authorization.UserAccess.Dtos;
using Generic.Domain.Entities;

namespace Auth.Application.Authorization.UserAccess.Interfaces;

public interface IUseCaseChangeUserEmail
{
    OperationResult Execute(ChangeUserEmailDto command);
}
