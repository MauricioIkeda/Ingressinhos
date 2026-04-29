using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Location.Dtos;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.Location.UseCases;

public class CreateLocationUseCase : IUseCaseCommand<LocationDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public CreateLocationUseCase(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }
    
    public bool Execute(LocationDto locationDto)
    {
        Messages.Clear();

        if (locationDto == null)
        {
            Messages.Add("Deve ser informado a localizacao", error: true);
            return false;
        }

        try
        {
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
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }
}
