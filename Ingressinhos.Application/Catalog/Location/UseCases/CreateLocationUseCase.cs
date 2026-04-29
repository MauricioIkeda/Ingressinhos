using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Location.Dtos;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.Location.UseCases;

public class CreateLocationUseCase : IUseCaseCommand<LocationDto>
{
    private readonly IRepositorySession _repositorySession;

    public CreateLocationUseCase(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }
    
    public OperationResult Execute(LocationDto locationDto)
    {
        if (locationDto == null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Location", "Deve ser informado a localizacao."));
        }

        try
        {
            var locationEntity = new LocationDomain(locationDto.Name, locationDto.TotalCapacity, locationDto.HasSeats)
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            if (!locationEntity.IsValid)
            {
                return locationEntity.ToUnprocessableEntityResult();
            }
            
            var repository = _repositorySession.GetRepository();
            repository.Include(locationEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
