using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Domain.Sales.Entities;

public class OrderItem : BaseEntity
{
    public long OrderId { get; private set; }
    public long TicketId { get; private set; }
    public string TicketName { get; private set; }
    public int Quantity { get; private set; }
    public Price UnitPrice { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice.Value;

    protected OrderItem()
    {
        
    }
    
    public OrderItem(long orderId, long ticketId, string ticketName, int quantity, decimal unitPrice)
    {
        if (orderId <= 0)
        {
            throw new Exception("Deve ser informado o pedido do item");
        }

        if (ticketId <= 0)
        {
            throw new Exception("Deve ser informado o ingresso do item");
        }

        if (string.IsNullOrWhiteSpace(ticketName))
        {
            throw new Exception("Deve ser informado o nome do ingresso");
        }

        if (quantity <= 0)
        {
            throw new Exception("A quantidade deve ser maior que zero");
        }

        OrderId = orderId;
        TicketId = ticketId;
        TicketName = ticketName.Trim();
        Quantity = quantity;
        UnitPrice = new Price(unitPrice);
    }
}