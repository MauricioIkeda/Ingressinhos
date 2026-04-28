namespace Generic.Application.Utils.Interface
{
    public interface IRequestAuth
    {
        Task<string> CreateUser(string name, string email, string password, int role);
        Task<string> Authenticate(string email, string password);
        Task<bool> ChangeEmail(string userId, string newEmail);
    }
}
