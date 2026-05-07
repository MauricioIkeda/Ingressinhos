using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Domain.Entities;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Auth.Application.Authorization.UserAccess.UseCases
{
    public class ActivateUser : IUseCaseActivateUser
    {
        private readonly IRepositorySession _repositorySession;

        public ActivateUser(IRepositorySession repositorySession)
        {
            _repositorySession = repositorySession;
        }

        public OperationResult Execute(string userId)
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var user = repositoryQuery.Query<UserAuth>(u => u.UserId == userId).FirstOrDefault();
            if (user is null)
            {
                return OperationResult.NotFound(new MensagemErro("Usuario", "Nao encontramos a conta informada."));
            }

            if (user.Active)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Usuario", "A conta informada ja esta ativa."));
            }

            user.Activate();
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
