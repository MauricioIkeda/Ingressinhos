using Ingressinhos.Domain.Catalog.Enums;
using Generic.Domain.Entities;

namespace Ingressinhos.Domain.Catalog.Entities;

public class PublishedTicket : BaseEntity
{
    public Guid TicketId { get; private set; }
    public Guid? SeatId { get; private set; }
    public SeatCategory Category { get; private set; }
    public SeatStatus SeatAvailabilityStatus { get; private set; }
    public decimal UnitPrice { get; private set; }
    public DateTime PublishedAt { get; private set; }
    public DateTime? ReservedAt { get; private set; }
    public DateTime? OccupiedAt { get; private set; }

    public Guid PublishedTicketId => Id;

    public PublishedTicket(Guid ticketId, Guid? seatId, SeatCategory category, decimal unitPrice)
    {
        if (ticketId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o ingresso publicado");
        }

        if (unitPrice < 0)
        {
            throw new Exception("O preco nao pode ser negativo");
        }

        Id = Guid.NewGuid();
        TicketId = ticketId;
        SeatId = seatId;
        Category = category;
        UnitPrice = unitPrice;
        SeatAvailabilityStatus = SeatStatus.Available;
        PublishedAt = DateTime.UtcNow;
    }

    public void Reserve()
    {
        if (SeatAvailabilityStatus == SeatStatus.Blocked)
        {
            throw new Exception("Bilhete publicado bloqueado nao pode ser reservado");
        }

        if (SeatAvailabilityStatus == SeatStatus.Occupied)
        {
            throw new Exception("Bilhete publicado ocupado nao pode ser reservado");
        }

        SeatAvailabilityStatus = SeatStatus.Reserved;
        ReservedAt = DateTime.UtcNow;
    }

    public void Occupy()
    {
        if (SeatAvailabilityStatus == SeatStatus.Blocked)
        {
            throw new Exception("Bilhete publicado bloqueado nao pode ser ocupado");
        }

        SeatAvailabilityStatus = SeatStatus.Occupied;
        OccupiedAt = DateTime.UtcNow;
    }

    public void Release()
    {
        if (SeatAvailabilityStatus == SeatStatus.Blocked)
        {
            throw new Exception("Bilhete publicado bloqueado nao pode ser liberado");
        }

        SeatAvailabilityStatus = SeatStatus.Available;
        ReservedAt = null;
    }

    public void Block()
    {
        SeatAvailabilityStatus = SeatStatus.Blocked;
    }

    public void Unblock()
    {
        SeatAvailabilityStatus = SeatStatus.Available;
    }

    public void ChangePrice(decimal unitPrice)
    {
        if (unitPrice < 0)
        {
            throw new Exception("O preco nao pode ser negativo");
        }

        UnitPrice = unitPrice;
    }
}