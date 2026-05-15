using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Payment.Aplication.Transactions.Dtos;
using Payment.Domain.Entities;

namespace Payment.Aplication.Transactions.Interfaces;

public interface IUseCasePaymentTransactionCollection : IUseCaseQueryCollection<PaymentTransaction>
{
    OperationResult<PaymentCheckoutDto> Request(RequestPaymentDto command);
    OperationResult<IEnumerable<PaymentTransactionDto>> GetByOrder(long orderId);
    OperationResult<PaymentTransactionDto> CheckStatus(long orderId);
    OperationResult<List<TOutput>> GetQueryItems<TOutput>(Func<IQueryable<PaymentTransactionQueryItem>, IQueryable<TOutput>> transaction);
}
