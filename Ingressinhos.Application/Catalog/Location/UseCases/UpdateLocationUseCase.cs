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
    private readonly IRepositorySession _repositorySession;

    public UpdateLocationUseCase(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }
    
    public OperationResult Execute(LocationDto locationDto)
    {
        if (locationDto == null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Location", "Deve ser informado o que mudar."));
        }

        if (locationDto.Id <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado qual localizacao mudar."));
        }
        
        try
        {
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();
            LocationDomain locationEntity = repositoryQuery.Return<LocationDomain>(locationDto.Id);

            if (locationEntity == null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Nao foi encontrado essa localizacao."));
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
                        return OperationResult.UnprocessableEntity(new MensagemErro("HasSeats", "Nao e possivel desativar os assentos, pois existem assentos associados a esta localizacao."));
                    }
                }

                locationEntity.ChangeSeatMode(locationDto.HasSeats);
            }
            
            _repositorySession.GetRepository().Merge(locationEntity);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
