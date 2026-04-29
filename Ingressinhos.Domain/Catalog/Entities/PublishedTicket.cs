using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Domain.Catalog.Entities;

public class PublishedTicket : BaseEntity
{
    public long TicketId { get; private set; }
    public long? SeatId { get; private set; }
    public SeatCategory Category { get; private set; }
    public SeatStatus SeatAvailabilityStatus { get; private set; }
    public Price UnitPrice { get; private set; } = new(0);
    public DateTime PublishedAt { get; private set; }
    public DateTime? ReservedAt { get; private set; }
    public DateTime? OccupiedAt { get; private set; }

    protected PublishedTicket()
    {
    }

    public long PublishedTicketId => Id;

    public PublishedTicket(long ticketId, long? seatId, SeatCategory category, decimal unitPrice)
    {
        if (ticketId <= 0)
        {
            AddError("TicketId", "Deve ser informado o ingresso publicado");
        }
        else
        {
            TicketId = ticketId;
        }

        SeatId = seatId;
        Category = category;

        var price = new Price(unitPrice);
        CopyErrorsFrom(price);
        if (price.IsValid)
        {
            UnitPrice = price;
        }

        SeatAvailabilityStatus = SeatStatus.Available;
        PublishedAt = DateTime.UtcNow;
    }

    public void Reserve()
    {
        ClearErrors();

        if (SeatAvailabilityStatus == SeatStatus.Blocked)
        {
            AddError("Status", "Bilhete publicado bloqueado nao pode ser reservado");
            return;
        }

        if (SeatAvailabilityStatus == SeatStatus.Occupied)
        {
            AddError("Status", "Bilhete publicado ocupado nao pode ser reservado");
            return;
        }

        SeatAvailabilityStatus = SeatStatus.Reserved;
        ReservedAt = DateTime.UtcNow;
    }

    public void Occupy()
    {
        ClearErrors();

        if (SeatAvailabilityStatus == SeatStatus.Blocked)
        {
            AddError("Status", "Bilhete publicado bloqueado nao pode ser ocupado");
            return;
        }

        SeatAvailabilityStatus = SeatStatus.Occupied;
        OccupiedAt = DateTime.UtcNow;
    }

    public void Release()
    {
        ClearErrors();

        if (SeatAvailabilityStatus == SeatStatus.Blocked)
        {
            AddError("Status", "Bilhete publicado bloqueado nao pode ser liberado");
            return;
        }

        SeatAvailabilityStatus = SeatStatus.Available;
        ReservedAt = null;
    }

    public void Block()
    {
        ClearErrors();
        SeatAvailabilityStatus = SeatStatus.Blocked;
    }

    public void Unblock()
    {
        ClearErrors();
        SeatAvailabilityStatus = SeatStatus.Available;
    }

    public void ChangePrice(decimal unitPrice)
    {
        ClearErrors();

        var price = new Price(unitPrice);
        CopyErrorsFrom(price);
        if (!price.IsValid)
        {
            return;
        }

        UnitPrice = price;
    }
}
