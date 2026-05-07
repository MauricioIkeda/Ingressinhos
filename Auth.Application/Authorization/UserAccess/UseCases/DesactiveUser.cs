using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Auth.Domain.Entities;
using Auth.Application.Authorization.UserAccess.Interfaces;

namespace Auth.Application.Authorization.UserAccess.UseCases
{
    public class DesactiveUser : IUseCaseDesactiveUser
    {
        private IRepositorySession _repositorySession;
        public DesactiveUser(IRepositorySession repositorySession)
        {
            _repositorySession = repositorySession;
        }

        public OperationResult Execute(string userId)
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var user = repositoryQuery.Query<UserAuth>(u => u.UserId == userId && u.Active).FirstOrDefault();
            if (user is null)
            {
                return OperationResult.NotFound(new MensagemErro("Usuario", "Nao encontramos a conta informada."));
            }

            user.Deactivate();
            if (!user.IsValid)
            {
                return OperationResult.UnprocessableEntity(user.Errors);
            }

            var repository = _repositorySession.GetRepository();
            repository.Upsert(user);
            repository.Flush().GetAwaiter().GetResult();

            return OperationResult.Ok();
        }
    }
}
