using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SeatInclude
{
    private readonly IRepositorySession _repositorySession;

    public SeatInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(SeatDto seat)
    {
        if (seat is null)
        {
            throw new Exception("Deve ser informado o assento");
        }

        var utcNow = DateTime.UtcNow;

        IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();
        LocationDomain location = repositoryQuery.Return<LocationDomain>(seat.LocationId);
        if (location is null)
        {
            throw new Exception("Local não encontrado");
        }

        if (!location.HasSeats)
        {
            throw new Exception("O local informado não possui assentos");
        }

        var existingSeat = repositoryQuery.Count<Seat>(s => s.LocationId == seat.LocationId && s.Code == seat.Code) > 0;
        if (existingSeat)
        {
            throw new Exception("Já existe um assento com o mesmo código neste local");
        }

        var seatEntity = new Seat(seat.LocationId, seat.Code, seat.Category)
        {
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        ApplySeatStatus(seatEntity, seat.Status);

        var repository = _repositorySession.GetRepository();
        repository.Include(seatEntity);
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