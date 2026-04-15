using Generic.Domain.Entities;
using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Domain.Sales.Entities;

public class IssuedTicket : BaseEntity
{
    public Guid OrderItemId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid EventId { get; private set; }
    public string AccessCode { get; private set; }
    public IssuedTicketStatus Status { get; private set; }
    public bool IsCheckedIn => Status == IssuedTicketStatus.CheckedIn;
    public DateTime IssuedAt { get; private set; }
    public DateTime? CheckedInAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public IssuedTicket(Guid orderItemId, Guid clientId, Guid eventId, string accessCode)
    {
        if (orderItemId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o item do pedido");
        }

        if (clientId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o cliente");
        }

        if (eventId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o evento");
        }

        if (string.IsNullOrWhiteSpace(accessCode))
        {
            throw new Exception("Deve ser informado o codigo de acesso do ingresso");
        }

        Id = Guid.NewGuid();
        OrderItemId = orderItemId;
        ClientId = clientId;
        EventId = eventId;
        AccessCode = accessCode.Trim();
        Status = IssuedTicketStatus.Issued;
        IssuedAt = DateTime.UtcNow;
    }

    public void CheckIn()
    {
        if (Status == IssuedTicketStatus.CheckedIn)
        {
            throw new Exception("O ingresso ja foi utilizado no check-in");
        }

        if (Status == IssuedTicketStatus.Cancelled)
        {
            throw new Exception("Nao eh possivel fazer check-in com bilhete cancelado");
        }

        Status = IssuedTicketStatus.CheckedIn;
        CheckedInAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == IssuedTicketStatus.CheckedIn)
        {
            throw new Exception("Nao eh possivel cancelar um bilhete ja utilizado");
        }

        if (Status == IssuedTicketStatus.Cancelled)
        {
            throw new Exception("O bilhete ja esta cancelado");
        }

        Status = IssuedTicketStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }
}