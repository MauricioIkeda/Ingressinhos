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
            AddError("Pedido", "Informe o pedido deste item.");
        }
        else
        {
            OrderId = orderId;
        }

        if (ticketId <= 0)
        {
            AddError("Ingresso", "Informe o ingresso deste item.");
        }
        else
        {
            TicketId = ticketId;
        }

        if (string.IsNullOrWhiteSpace(ticketName))
        {
            AddError("Ingresso", "Informe o nome do ingresso.");
        }
        else
        {
            TicketName = ticketName.Trim();
        }

        if (quantity <= 0)
        {
            AddError("Quantidade", "A quantidade precisa ser maior que zero.");
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
