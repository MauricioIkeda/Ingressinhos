using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Onboarding.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Onboarding.UseCases;

public sealed class OnboardClientUseCase
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;

    public OnboardClientUseCase(
        ICurrentUserContext currentUserContext,
        IRepositorySession repositorySession,
        IRequestAuth requestAuth)
    {
        _currentUserContext = currentUserContext;
        _repositorySession = repositorySession;
        _requestAuth = requestAuth;
    }

    public OperationResult Execute(OnboardClientRequest request)
    {
        if (!_currentUserContext.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserContext.UserId))
        {
            return OperationResult.Unauthorized(new MensagemErro("Usuario", "Usuario nao autenticado."));
        }

        if (request is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Cliente", "Informe os dados do cliente."));
        }

        var query = _repositorySession.GetRepositoryQuery();
        if (query.Query<ClientDomain>(client => client.UserId == _currentUserContext.UserId).Any() ||
            query.Query<Seller>(seller => seller.UserId == _currentUserContext.UserId).Any())
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Perfil", "Usuario ja possui perfil no Ingressinhos."));
        }

        var utcNow = DateTime.UtcNow;
        var clientEntity = new ClientDomain(
            request.Name,
            request.Email,
            request.Cpf,
            _currentUserContext.UserId
        )
        {
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        if (!clientEntity.IsValid)
        {
            return clientEntity.ToUnprocessableEntityResult();
        }

        var assignRole = _requestAuth.AssignRole(_currentUserContext.UserId, 2).GetAwaiter().GetResult();
        if (!assignRole.Success)
        {
            return assignRole;
        }

        var repository = _repositorySession.GetRepository();
        repository.Include(clientEntity);
        repository.Flush().GetAwaiter().GetResult();

        return OperationResult.Created();
    }
}
