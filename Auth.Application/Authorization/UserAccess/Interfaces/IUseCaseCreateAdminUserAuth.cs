using Auth.Application.Authorization.UserAccess.Dtos;
using Generic.Domain.Entities;

namespace Auth.Application.Authorization.UserAccess.Interfaces;

public interface IUseCaseCreateAdminUserAuth
{
    OperationResult<string> Execute(CreateAdminRequest command, bool isAuthenticated, bool isAdmin);
}
