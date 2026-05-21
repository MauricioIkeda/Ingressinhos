using Generic.Application.Dtos;
using Generic.Domain.Entities;
using Ingressinhos.Application.Sales.Dtos;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseImmediateOrder
{
    OperationResult<PaymentCheckoutApiDto> Execute(CreateOrderRequest command);
}
