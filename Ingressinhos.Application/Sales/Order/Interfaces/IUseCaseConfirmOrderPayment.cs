using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseConfirmOrderPayment // usado somente em worker, confirma pagamento e cria ingressos
{
    OperationResult Execute(long orderId);
}
