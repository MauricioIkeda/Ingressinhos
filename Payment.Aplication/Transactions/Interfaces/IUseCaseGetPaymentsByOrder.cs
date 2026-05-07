using Generic.Domain.Entities;
using Payment.Aplication.Transactions.Dtos;

namespace Payment.Aplication.Transactions.Interfaces;

public interface IUseCaseGetPaymentsByOrder
{
    OperationResult<IEnumerable<PaymentTransactionDto>> Execute(long orderId);
}
