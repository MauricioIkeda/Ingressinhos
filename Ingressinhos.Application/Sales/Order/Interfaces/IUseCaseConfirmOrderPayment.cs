using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseConfirmOrderPayment
{
    OperationResult Execute(long orderId);
}
