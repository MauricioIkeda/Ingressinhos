using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Location.Dtos;

namespace Ingressinhos.Application.Catalog.Location.UseCases;

public class CreateLocationUseCase
{
    private readonly IRepositorySession _repositorySession;

    public CreateLocationUseCase(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }
    
    public Domain.Catalog.Entities.Location Execute(LocationDto locationDto)
    {
        if (locationDto == null)
        {
            throw new Exception("Deve ser informado a localizacao");
        }

        var locationEntity = new Domain.Catalog.Entities.Location(locationDto.Name, locationDto.TotalCapacity, locationDto.HasSeats)
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var repository = _repositorySession.GetRepository();
        repository.Include(locationEntity);
        repository.Flush().GetAwaiter().GetResult();
        return locationEntity;
    }
}