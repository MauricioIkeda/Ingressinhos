using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Payment.Aplication.Refunds.Dtos;
using Payment.Domain.Entities;

namespace Payment.Aplication.Refunds.Interfaces;

public interface IUseCaseRefundCollection : IUseCaseQueryCollection<Refund>
{
    OperationResult<RefundDto> Request(RequestRefundDto command);
    OperationResult<IEnumerable<RefundDto>> GetByPaymentTransaction(long paymentTransactionId);
    OperationResult<RefundDto> CheckStatus(long refundId);
    OperationResult<List<TOutput>> GetQueryItems<TOutput>(Func<IQueryable<RefundQueryItem>, IQueryable<TOutput>> transaction);
}
