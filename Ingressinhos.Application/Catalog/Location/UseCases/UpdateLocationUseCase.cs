using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Location.Dtos;

namespace Ingressinhos.Application.Catalog.Location.UseCases;

public class UpdateLocationUseCase
{
    private IRepositorySession _repositorySession;

    public UpdateLocationUseCase(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }
    
    public bool Execute(LocationDto locationDto)
    {
        if (locationDto == null)
        {
            throw new Exception("Deve ser informado o que mudar");
        }

        if (locationDto.Id <= 0)
        {
            throw new Exception("Deve ser informado qual Localiza��o mudar");
        }
        
        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var locationEntity = repositoryQuery.Return<Domain.Catalog.Entities.Location>(locationDto.Id);

        if (locationEntity == null)
        {
            throw new Exception("N�o foi encontrado essa Localiza��o");
        }
        
        if (locationDto.Name != locationEntity.Name)
        {
            locationEntity.ChangeName(locationDto.Name);
        }

        if (locationDto.TotalCapacity != locationEntity.TotalCapacity)
        {
            locationEntity.ChangeTotalCapacity(locationDto.TotalCapacity);
        }

        if (locationDto.HasSeats != locationEntity.HasSeats)
        {
            locationEntity.ChangeSeatMode(locationDto.HasSeats);
        }
        
        _repositorySession.GetRepository().Merge(locationEntity);
        return true;
    }
}