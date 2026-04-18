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
    
    public Domain.Catalog.Entities.Location Execute(LocationUpdateDto locationUpdateDto)
    {
        if (locationUpdateDto == null)
        {
            throw new Exception("Deve ser informado o que mudar");
        }

        if (locationUpdateDto.Id <= 0)
        {
            throw new Exception("Deve ser informado qual Location mudar");
        }
        
        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var locationEntity = repositoryQuery.Return<Domain.Catalog.Entities.Location>(locationUpdateDto.Id);

        if (locationEntity == null)
        {
            throw new Exception("Nao foi encontrado essa Localizacao");
        }
        
        if (locationUpdateDto.Name != null)
        {
            locationEntity.ChangeName(locationUpdateDto.Name);
        }

        if (locationUpdateDto.TotalCapacity != null)
        {
            locationEntity.ChangeTotalCapacity(locationUpdateDto.TotalCapacity.Value);
        }

        if (locationUpdateDto.HasSeats != null)
        {
            locationEntity.ChangeSeatMode(locationUpdateDto.HasSeats.Value);
        }
        
        return locationEntity;
    }
}