using Auth.Application.Authorization.UserAccess.Dtos;

namespace Auth.Application.Authorization.UserAccess.Interfaces;

public interface IUseCaseUserAccessQuery
{
    UserAccessDto Execute(long userId);
}