using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;

namespace Ingressinhos.Domain.Sales.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid TicketId { get; private set; }
    public string TicketName { get; private set; }
    public int Quantity { get; private set; }
    public Price UnitPrice { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice.Value;

    public OrderItem(Guid orderId, Guid ticketId, string ticketName, int quantity, decimal unitPrice)
    {
        if (orderId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o pedido do item");
        }

        if (ticketId == Guid.Empty)
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

        Id = Guid.NewGuid();
        OrderId = orderId;
        TicketId = ticketId;
        TicketName = ticketName.Trim();
        Quantity = quantity;
        UnitPrice = new Price(unitPrice);
    }
}