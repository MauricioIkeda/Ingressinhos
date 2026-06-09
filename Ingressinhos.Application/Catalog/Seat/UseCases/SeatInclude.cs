using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SeatInclude : IUseCaseCommand<SeatDto>
{
    private readonly IRepositorySession _repositorySession;

    public SeatInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(SeatDto seat)
    {
        if (seat is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Seat", "Deve ser informado o assento."));
        }

        if (seat.Status is not SeatStatus.Available and not SeatStatus.Blocked)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Status", "O status do assento deve representar apenas disponibilidade fisica: disponivel ou bloqueado."));
        }

        try
        {
            var utcNow = DateTime.UtcNow;

            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();
            LocationDomain location = repositoryQuery.Return<LocationDomain>(seat.LocationId);
            if (location is null)
            {
                return OperationResult.NotFound(new MensagemErro("LocationId", "Local nao encontrado."));
            }

            if (!location.HasSeats)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("LocationId", "O local informado nao possui assentos."));
            }

            var existingSeat = repositoryQuery.Count<Seat>(s => s.LocationId == seat.LocationId && s.Code == seat.Code) > 0;
            if (existingSeat)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Code", "Ja existe um assento com o mesmo codigo neste local."));
            }

            var seatEntity = new Seat(seat.LocationId, seat.Code, seat.Category)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
            if (!seatEntity.IsValid)
            {
                return seatEntity.ToUnprocessableEntityResult();
            }

            ApplySeatStatus(seatEntity, seat.Status);
            if (!seatEntity.IsValid)
            {
                return seatEntity.ToUnprocessableEntityResult();
            }

            var repository = _repositorySession.GetRepository();
            repository.Include(seatEntity);
            repository.Flush();
            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

    private static void ApplySeatStatus(Seat seatEntity, SeatStatus status)
    {
        if (status == SeatStatus.Blocked)
        {
            seatEntity.Block();
            return;
        }

        if (seatEntity.Status == SeatStatus.Blocked)
        {
            seatEntity.Unblock();
            return;
        }

        if (seatEntity.Status == SeatStatus.Reserved || seatEntity.Status == SeatStatus.Occupied)
        {
            seatEntity.Release();
        }
    }
}
