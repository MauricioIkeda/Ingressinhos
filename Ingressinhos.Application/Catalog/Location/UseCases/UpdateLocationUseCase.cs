using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Location.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.Location.UseCases;

public class UpdateLocationUseCase : IUseCaseCommand<LocationDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public UpdateLocationUseCase(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }
    
    public bool Execute(LocationDto locationDto)
    {
        Messages.Clear();

        if (locationDto == null)
        {
            Messages.Add("Deve ser informado o que mudar", error: true);
            return false;
        }

        if (locationDto.Id <= 0)
        {
            Messages.Add("Deve ser informado qual Localizacao mudar", error: true);
            return false;
        }
        
        try
        {
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();
            LocationDomain locationEntity = repositoryQuery.Return<LocationDomain>(locationDto.Id);

            if (locationEntity == null)
            {
                Messages.Add("Nao foi encontrado essa Localizacao", error: true);
                return false;
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
                        Messages.Add("Nao e possivel desativar os assentos, pois existem assentos associados a esta localizacao", error: true);
                        return false;
                    }
                }

                locationEntity.ChangeSeatMode(locationDto.HasSeats);
            }
            
            _repositorySession.GetRepository().Merge(locationEntity);
            return true;
        }
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }
}
