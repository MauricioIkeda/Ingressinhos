using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Onboarding.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Onboarding.UseCases;

public sealed class ProfileStatusUseCase
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IReadRepositoryQuery _query;

    public ProfileStatusUseCase(ICurrentUserContext currentUserContext, IReadRepositoryQuery query)
    {
        _currentUserContext = currentUserContext;
        _query = query;
    }

    public OperationResult<ProfileStatusDto> Execute()
    {
        if (!_currentUserContext.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserContext.UserId))
        {
            return OperationResult<ProfileStatusDto>.Unauthorized(
                new MensagemErro("Usuario", "Usuario nao autenticado.")
            );
        }

        var client = _query
            .Query<ClientDomain>(client => client.UserId == _currentUserContext.UserId)
            .FirstOrDefault();

        var seller = _query
            .Query<Seller>(seller => seller.UserId == _currentUserContext.UserId)
            .FirstOrDefault();

        return OperationResult<ProfileStatusDto>.Ok(
            new ProfileStatusDto(
                client is not null,
                seller is not null,
                client?.Id,
                seller?.Id
            )
        );
    }
}
