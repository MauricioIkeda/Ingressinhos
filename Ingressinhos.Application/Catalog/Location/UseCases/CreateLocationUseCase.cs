using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Location.Dtos;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.Location.UseCases;

public class CreateLocationUseCase
{
    private readonly IRepositorySession _repositorySession;

    public CreateLocationUseCase(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }
    
    public bool Execute(LocationDto locationDto)
    {
        if (locationDto == null)
        {
            throw new Exception("Deve ser informado a localização");
        }

        var locationEntity = new LocationDomain(locationDto.Name, locationDto.TotalCapacity, locationDto.HasSeats)
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var repository = _repositorySession.GetRepository();
        repository.Include(locationEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}