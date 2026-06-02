using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseCancelOrderPayment // usado somente no worker para cancelar o pedido e devolver tudo reservado
{
    OperationResult Execute(long orderId);
}
