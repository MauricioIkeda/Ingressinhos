using Generic.Domain.Entities;
using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Domain.Sales.Entities;

public class SeatReservation : BaseEntity
{
    public long EventId { get; private set; }
    public long SeatId { get; private set; }
    public long OrderId { get; private set; }
    public long OrderItemId { get; private set; }
    public SeatReservationStatus Status { get; private set; }
    public DateTime ReservedAt { get; private set; }
    public DateTime? OccupiedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    protected SeatReservation()
    {
    }

    public SeatReservation(long eventId, long seatId, long orderId, long orderItemId)
    {
        if (eventId <= 0)
        {
            AddError("EventId", "Deve ser informado o evento da reserva.");
        }
        else
        {
            EventId = eventId;
        }

        if (seatId <= 0)
        {
            AddError("SeatId", "Deve ser informado o assento da reserva.");
        }
        else
        {
            SeatId = seatId;
        }

        if (orderId <= 0)
        {
            AddError("OrderId", "Deve ser informado o pedido da reserva.");
        }
        else
        {
            OrderId = orderId;
        }

        if (orderItemId <= 0)
        {
            AddError("OrderItemId", "Deve ser informado o item do pedido da reserva.");
        }
        else
        {
            OrderItemId = orderItemId;
        }

        Status = SeatReservationStatus.Reserved;
        ReservedAt = DateTime.UtcNow;
    }

    public void Occupy()
    {
        ClearErrors();

        if (Status == SeatReservationStatus.Cancelled)
        {
            AddError("Status", "Reserva cancelada nao pode ser ocupada.");
            return;
        }

        if (Status == SeatReservationStatus.Occupied)
        {
            return;
        }

        Status = SeatReservationStatus.Occupied;
        OccupiedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        ClearErrors();

        if (Status == SeatReservationStatus.Cancelled)
        {
            return;
        }

        Status = SeatReservationStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }
}
