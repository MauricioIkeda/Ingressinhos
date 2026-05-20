using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseCancelOrderPayment
{
    OperationResult Execute(long orderId);
}
