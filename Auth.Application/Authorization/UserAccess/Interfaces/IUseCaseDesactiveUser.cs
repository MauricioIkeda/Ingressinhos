using Generic.Domain.Entities;

namespace Auth.Application.Authorization.UserAccess.Interfaces
{
    public interface IUseCaseDesactiveUser
    {
        OperationResult Execute(string userId);
    }
}
