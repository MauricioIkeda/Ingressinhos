using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SeatUpdate
{
    private readonly IRepositorySession _repositorySession;

    public SeatUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(SeatDto seat)
    {
        if (seat is null)
        {
            throw new Exception("Deve ser informado o assento");
        }

        if (seat.SeatId <= 0)
        {
            throw new Exception("Deve ser informado o identificador do assento");
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var seatEntity = repositoryQuery.Return<Seat>(seat.SeatId);

        if (seatEntity is null)
        {
            throw new Exception("Assento nao encontrado");
        }

        if (seat.Category != seatEntity.Category)
        {
            seatEntity.ChangeCategory(seat.Category);
        }

        if (seat.Status != seatEntity.Status)
        {
            ApplySeatStatus(seatEntity, seat.Status);
        }

        seatEntity.UpdatedAt = DateTime.UtcNow;

        var repository = _repositorySession.GetRepository();
        repository.Upsert(seatEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
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