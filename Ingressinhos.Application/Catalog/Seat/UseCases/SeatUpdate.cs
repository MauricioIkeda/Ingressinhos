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
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public SeatUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(SeatDto seat)
    {
        Messages.Clear();

        if (seat is null)
        {
            Messages.Add("Deve ser informado o assento", error: true);
            return false;
        }

        if (seat.SeatId <= 0)
        {
            Messages.Add("Deve ser informado o identificador do assento", error: true);
            return false;
        }

        try
        {
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();
            Seat seatEntity = repositoryQuery.Return<Seat>(seat.SeatId);

            if (seatEntity is null)
            {
                Messages.Add("Assento nao encontrado", error: true);
                return false;
            }

            if (seat.Category != seatEntity.Category)
            {
                seatEntity.ChangeCategory(seat.Category);
            }

            if (seat.Status != seatEntity.Status)
            {
                if (seat.Status != SeatStatus.Blocked)
                {
                    LocationDomain location = repositoryQuery.Return<LocationDomain>(seatEntity.LocationId);
                    if (location.HasSeats == false)
                    {
                        Messages.Add("Nao e possivel alterar o status do assento, pois a localizacao nao possui assentos disponiveis", error: true);
                        return false;
                    }
                }

                ApplySeatStatus(seatEntity, seat.Status);
            }

            seatEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(seatEntity);
            repository.Flush().GetAwaiter().GetResult();
            return true;
        }
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
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
