using Generic.Application.Dtos;
using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseCloseOrder
{
    OperationResult<PaymentCheckoutApiDto> Execute(long orderId);
}
