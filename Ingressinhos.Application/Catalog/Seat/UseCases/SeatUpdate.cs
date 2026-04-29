using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SeatUpdate : IUseCaseCommand<SeatDto>
{
    private readonly IRepositorySession _repositorySession;

    public SeatUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(SeatDto seat)
    {
        if (seat is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Seat", "Deve ser informado o assento."));
        }

        if (seat.SeatId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do assento."));
        }

        try
        {
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();
            Seat seatEntity = repositoryQuery.Return<Seat>(seat.SeatId);

            if (seatEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Assento nao encontrado."));
            }

            if (seat.Category != seatEntity.Category)
            {
                seatEntity.ChangeCategory(seat.Category);
                if (!seatEntity.IsValid)
                {
                    return seatEntity.ToUnprocessableEntityResult();
                }
            }

            if (seat.Status != seatEntity.Status)
            {
                if (seat.Status != SeatStatus.Blocked)
                {
                    LocationDomain location = repositoryQuery.Return<LocationDomain>(seatEntity.LocationId);
                    if (location.HasSeats == false)
                    {
                        return OperationResult.UnprocessableEntity(new MensagemErro("Status", "Nao e possivel alterar o status do assento, pois a localizacao nao possui assentos disponiveis."));
                    }
                }

                ApplySeatStatus(seatEntity, seat.Status);
                if (!seatEntity.IsValid)
                {
                    return seatEntity.ToUnprocessableEntityResult();
                }
            }

            seatEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(seatEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Ok();
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
        }

        if (status == SeatStatus.Available)
        {
            if (seatEntity.Status == SeatStatus.Reserved || seatEntity.Status == SeatStatus.Occupied)
            {
                seatEntity.Release();
            }

            return;
        }

        if (status == SeatStatus.Reserved)
        {
            if (seatEntity.Status == SeatStatus.Occupied)
            {
                seatEntity.Release();
            }

            seatEntity.Reserve();
            return;
        }

        if (status == SeatStatus.Occupied)
        {
            seatEntity.Occupy();
        }
    }
}
