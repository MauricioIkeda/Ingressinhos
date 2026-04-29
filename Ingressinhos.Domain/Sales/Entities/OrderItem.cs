using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;

namespace Ingressinhos.Domain.Sales.Entities;

public class OrderItem : BaseEntity
{
    public long OrderId { get; private set; }
    public long TicketId { get; private set; }
    public string TicketName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Price UnitPrice { get; private set; } = new(0);
    public decimal TotalPrice => Quantity * UnitPrice.Value;

    protected OrderItem()
    {
    }
    
    public OrderItem(long orderId, long ticketId, string ticketName, int quantity, decimal unitPrice)
    {
        if (orderId <= 0)
        {
            AddError("OrderId", "Deve ser informado o pedido do item");
        }
        else
        {
            OrderId = orderId;
        }

        if (ticketId <= 0)
        {
            AddError("TicketId", "Deve ser informado o ingresso do item");
        }
        else
        {
            TicketId = ticketId;
        }

        if (string.IsNullOrWhiteSpace(ticketName))
        {
            AddError("TicketName", "Deve ser informado o nome do ingresso");
        }
        else
        {
            TicketName = ticketName.Trim();
        }

        if (quantity <= 0)
        {
            AddError("Quantity", "A quantidade deve ser maior que zero");
        }
        else
        {
            Quantity = quantity;
        }

        var price = new Price(unitPrice);
        CopyErrorsFrom(price);
        if (price.IsValid)
        {
            UnitPrice = price;
        }
    }
}
