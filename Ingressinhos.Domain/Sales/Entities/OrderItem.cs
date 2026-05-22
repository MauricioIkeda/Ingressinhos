using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Domain.Sales.Entities;

public class OrderItem : BaseEntity
{
    public long OrderId { get; private set; }
    public long TicketId { get; private set; }
    public string TicketName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Price UnitPrice { get; private set; } = new(0);
    public SeatCategory Category { get; private set; }
    public long? SeatId { get; private set; }
    public string? SeatCode { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice.Value;

    protected OrderItem()
    {
    }
    
    public OrderItem(long orderId, long ticketId, string ticketName, int quantity, decimal unitPrice, SeatCategory category, long? seatId = null, string? seatCode = null)
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

        Category = category;

        if (seatId.HasValue)
        {
            if (seatId.Value <= 0)
            {
                AddError("SeatId", "Informe um assento valido para o item.");
            }
            else
            {
                SeatId = seatId.Value;
            }

            if (quantity != 1)
            {
                AddError("Quantidade", "Itens com assento marcado devem ter quantidade 1.");
            }

            SeatCode = string.IsNullOrWhiteSpace(seatCode) ? null : seatCode.Trim().ToUpperInvariant();
        }
    }

    public void AddQuantity(int quantity)
    {
        ClearErrors();

        if (SeatId.HasValue)
        {
            AddError("Quantidade", "Itens com assento marcado nao podem agrupar quantidade.");
            return;
        }

        if (quantity <= 0)
        {
            AddError("Quantidade", "A quantidade precisa ser maior que zero.");
            return;
        }

        Quantity += quantity;
    }
}
