using Generic.Domain.Entities;
using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Domain.Sales.Entities;

public class IssuedTicket : BaseEntity
{
    public long OrderItemId { get; private set; }
    public long ClientId { get; private set; }
    public long EventId { get; private set; }
    public string AccessCode { get; private set; } = string.Empty;
    public IssuedTicketStatus Status { get; private set; }
    public bool IsCheckedIn => Status == IssuedTicketStatus.CheckedIn;
    public DateTime IssuedAt { get; private set; }
    public DateTime? CheckedInAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    protected IssuedTicket()
    {
    }

    public IssuedTicket(long orderItemId, long clientId, long eventId, string accessCode)
    {
        if (orderItemId <= 0)
        {
            AddError("OrderItemId", "Deve ser informado o item do pedido");
        }
        else
        {
            OrderItemId = orderItemId;
        }

        if (clientId <= 0)
        {
            AddError("ClientId", "Deve ser informado o cliente");
        }
        else
        {
            ClientId = clientId;
        }

        if (eventId <= 0)
        {
            AddError("EventId", "Deve ser informado o evento");
        }
        else
        {
            EventId = eventId;
        }

        if (string.IsNullOrWhiteSpace(accessCode))
        {
            AddError("AccessCode", "Deve ser informado o codigo de acesso do ingresso");
        }
        else
        {
            AccessCode = accessCode.Trim();
        }

        Status = IssuedTicketStatus.Issued;
        IssuedAt = DateTime.UtcNow;
    }

    public void CheckIn()
    {
        ClearErrors();

        if (Status == IssuedTicketStatus.CheckedIn)
        {
            AddError("Status", "O ingresso ja foi utilizado no check-in");
            return;
        }

        if (Status == IssuedTicketStatus.Cancelled)
        {
            AddError("Status", "Nao eh possivel fazer check-in com bilhete cancelado");
            return;
        }

        Status = IssuedTicketStatus.CheckedIn;
        CheckedInAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        ClearErrors();

        if (Status == IssuedTicketStatus.CheckedIn)
        {
            AddError("Status", "Nao eh possivel cancelar um bilhete ja utilizado");
            return;
        }

        if (Status == IssuedTicketStatus.Cancelled)
        {
            AddError("Status", "O bilhete ja esta cancelado");
            return;
        }

        Status = IssuedTicketStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }
}
