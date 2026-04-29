using Generic.Domain.Entities;

namespace Auth.Application.Authorization.UserAccess.Interfaces;

public interface IUseCaseUserAuthCollection
{
    OperationResult<string> Execute(string email, string password);
}
