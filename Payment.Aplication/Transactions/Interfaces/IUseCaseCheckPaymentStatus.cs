using Generic.Domain.Entities;
using Payment.Aplication.Transactions.Dtos;

namespace Payment.Aplication.Transactions.Interfaces;

public interface IUseCaseCheckPaymentStatus
{
    OperationResult<PaymentTransactionDto> Execute(long orderId);
}
