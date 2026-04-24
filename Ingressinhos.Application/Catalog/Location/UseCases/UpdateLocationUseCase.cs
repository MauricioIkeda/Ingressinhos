using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Location.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

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
            throw new Exception("Deve ser informado qual Localização mudar");
        }
        
        IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();
        LocationDomain locationEntity = repositoryQuery.Return<LocationDomain>(locationDto.Id);

        if (locationEntity == null)
        {
            throw new Exception("Não foi encontrado essa Localização");
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
            if (!locationDto.HasSeats)
            {
                int totalSeats = repositoryQuery.Count<Seat>(s => s.LocationId == locationEntity.Id && s.Status != SeatStatus.Blocked);
                if (totalSeats > 0)
                {
                    throw new Exception("Não é possível desativar os assentos, pois existem assentos associados a esta localização");
                }
            }
            locationEntity.ChangeSeatMode(locationDto.HasSeats);
        }
        
        _repositorySession.GetRepository().Merge(locationEntity);
        return true;
    }
}