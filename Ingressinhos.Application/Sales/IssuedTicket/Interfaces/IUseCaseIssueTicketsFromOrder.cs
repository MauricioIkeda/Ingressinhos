using Generic.Domain.Entities;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseIssueTicketsFromOrder
{
    OperationResult Execute(long orderId);
}
