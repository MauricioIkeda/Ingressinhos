using Generic.Application.Dtos;
using Generic.Domain.Entities;

namespace Generic.Application.Utils.Interface
{
    public interface IRequestAuth
    {
        Task<OperationResult<string>> CreateUser(string name, string email, string password, int role);
        Task<OperationResult> ChangeEmail(string userId, string newEmail);
        Task<OperationResult> DeactivateUser(string userId);
        Task<OperationResult> ActivateUser(string userId);
    }
}
