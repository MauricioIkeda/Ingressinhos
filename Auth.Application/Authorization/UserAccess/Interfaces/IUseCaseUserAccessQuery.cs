using Auth.Application.Authorization.UserAccess.Dtos;
using Generic.Domain.Entities;

namespace Auth.Application.Authorization.UserAccess.Interfaces;

public interface IUseCaseUserAccessQuery
{
    OperationResult<UserAccessDto> Execute(string userId);
}
