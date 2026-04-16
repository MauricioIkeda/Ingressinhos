using Generic.Domain.Entities;
using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Domain.Sales.Entities;

public class Order : BaseEntity
{
    public Guid ClientId { get; private set; }
    public Client Client { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime OrderedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    protected Order()
    {
        
    }
    
    public Order(Guid clientId)
    {
        if (clientId == Guid.Empty)
        {
            throw new Exception("Deve ser informado o cliente do pedido");
        }

        Id = Guid.NewGuid();
        ClientId = clientId;
        Status = OrderStatus.PendingPayment;
        OrderedAt = DateTime.UtcNow;
        TotalAmount = 0;
    }

    public void AddItem(decimal unitPrice, int quantity)
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new Exception("Nao eh possivel alterar itens de um pedido finalizado");
        }

        if (unitPrice < 0)
        {
            throw new Exception("O valor unitario nao pode ser negativo");
        }

        if (quantity <= 0)
        {
            throw new Exception("A quantidade deve ser maior que zero");
        }

        TotalAmount += unitPrice * quantity;
    }

    public void ConfirmPayment()
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new Exception("Apenas pedidos pendentes podem ser pagos");
        }

        Status = OrderStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
        {
            throw new Exception("O pedido ja esta cancelado");
        }

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }
}