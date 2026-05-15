using Generic.Application.Crud.UseCases;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Refunds.Dtos;
using Payment.Aplication.Refunds.Interfaces;
using Payment.Domain.Entities;

namespace Payment.Aplication.Refunds.UseCases;

public class UseCaseRefundCollection : UseCaseQueryCollection<Refund>, IUseCaseRefundCollection
{
    private readonly IUseCaseRequestRefund _requestRefund;
    private readonly IUseCaseGetRefundsByPaymentTransaction _getRefundsByPaymentTransaction;
    private readonly IUseCaseCheckRefundStatus _checkRefundStatus;
    private readonly RefundGetOdata _refundGetOdata;

    public UseCaseRefundCollection(
        IRepositorySession repositorySession,
        IUseCaseRequestRefund requestRefund,
        IUseCaseGetRefundsByPaymentTransaction getRefundsByPaymentTransaction,
        IUseCaseCheckRefundStatus checkRefundStatus,
        RefundGetOdata refundGetOdata)
        : base(new UseCaseGetOdata<Refund>(), new UseCaseGetId<Refund>(), repositorySession)
    {
        _requestRefund = requestRefund;
        _getRefundsByPaymentTransaction = getRefundsByPaymentTransaction;
        _checkRefundStatus = checkRefundStatus;
        _refundGetOdata = refundGetOdata;
    }

    public OperationResult<RefundDto> Request(RequestRefundDto command)
    {
        return _requestRefund.Execute(command);
    }

    public OperationResult<IEnumerable<RefundDto>> GetByPaymentTransaction(long paymentTransactionId)
    {
        return _getRefundsByPaymentTransaction.Execute(paymentTransactionId);
    }

    public OperationResult<RefundDto> CheckStatus(long refundId)
    {
        return _checkRefundStatus.Execute(refundId);
    }

    public OperationResult<List<TOutput>> GetQueryItems<TOutput>(Func<IQueryable<RefundQueryItem>, IQueryable<TOutput>> transaction)
    {
        return _refundGetOdata.Execute(transaction);
    }
}
