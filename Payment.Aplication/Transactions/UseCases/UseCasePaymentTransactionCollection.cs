using Generic.Application.Crud.UseCases;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Domain.Entities;

namespace Payment.Aplication.Transactions.UseCases;

public class UseCasePaymentTransactionCollection : UseCaseQueryCollection<PaymentTransaction>, IUseCasePaymentTransactionCollection
{
    private readonly IUseCaseRequestPayment _requestPayment;
    private readonly IUseCaseGetPaymentsByOrder _getPaymentsByOrder;
    private readonly IUseCaseCheckPaymentStatus _checkPaymentStatus;
    private readonly PaymentTransactionGetOdata _paymentTransactionGetOdata;

    public UseCasePaymentTransactionCollection(
        IRepositorySession repositorySession,
        IUseCaseRequestPayment requestPayment,
        IUseCaseGetPaymentsByOrder getPaymentsByOrder,
        IUseCaseCheckPaymentStatus checkPaymentStatus,
        PaymentTransactionGetOdata paymentTransactionGetOdata)
        : base(new UseCaseGetOdata<PaymentTransaction>(), new UseCaseGetId<PaymentTransaction>(), repositorySession)
    {
        _requestPayment = requestPayment;
        _getPaymentsByOrder = getPaymentsByOrder;
        _checkPaymentStatus = checkPaymentStatus;
        _paymentTransactionGetOdata = paymentTransactionGetOdata;
    }

    public OperationResult<PaymentCheckoutDto> Request(RequestPaymentDto command)
    {
        return _requestPayment.Execute(command);
    }

    public OperationResult<IEnumerable<PaymentTransactionDto>> GetByOrder(long orderId)
    {
        return _getPaymentsByOrder.Execute(orderId);
    }

    public OperationResult<PaymentTransactionDto> CheckStatus(long orderId)
    {
        return _checkPaymentStatus.Execute(orderId);
    }

    public OperationResult<List<TOutput>> GetQueryItems<TOutput>(Func<IQueryable<PaymentTransactionQueryItem>, IQueryable<TOutput>> transaction)
    {
        return _paymentTransactionGetOdata.Execute(transaction);
    }
}
