using Generic.Domain.Entities;

namespace Auth.Application.Authorization.UserAccess.Interfaces
{
    public interface IUseCaseActivateUser
    {
        OperationResult Execute(string userId);
    }
}
