using Generic.Domain.Entities;
using Payment.Aplication.Refunds.Dtos;

namespace Payment.Aplication.Refunds.Interfaces;

public interface IUseCaseGetRefundsByPaymentTransaction
{
    OperationResult<IEnumerable<RefundDto>> Execute(long paymentTransactionId);
}
