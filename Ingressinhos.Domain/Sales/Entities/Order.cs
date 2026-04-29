using Generic.Domain.Entities;
using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Domain.Sales.Entities;

public class Order : BaseEntity
{
    public long ClientId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime OrderedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    protected Order()
    {
    }
    
    public Order(long clientId)
    {
        if (clientId <= 0)
        {
            AddError("ClientId", "Deve ser informado o cliente do pedido");
        }
        else
        {
            ClientId = clientId;
        }

        Status = OrderStatus.PendingPayment;
        OrderedAt = DateTime.UtcNow;
        TotalAmount = 0;
    }

    public void AddItem(decimal unitPrice, int quantity)
    {
        ClearErrors();

        if (Status != OrderStatus.PendingPayment)
        {
            AddError("Status", "Nao eh possivel alterar itens de um pedido finalizado");
            return;
        }

        if (unitPrice < 0)
        {
            AddError("UnitPrice", "O valor unitario nao pode ser negativo");
            return;
        }

        if (quantity <= 0)
        {
            AddError("Quantity", "A quantidade deve ser maior que zero");
            return;
        }

        TotalAmount += unitPrice * quantity;
    }

    public void ConfirmPayment()
    {
        ClearErrors();

        if (Status != OrderStatus.PendingPayment)
        {
            AddError("Status", "Apenas pedidos pendentes podem ser pagos");
            return;
        }

        Status = OrderStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        ClearErrors();

        if (Status == OrderStatus.Cancelled)
        {
            AddError("Status", "O pedido ja esta cancelado");
            return;
        }

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }
}
