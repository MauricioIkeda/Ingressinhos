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

    public UseCasePaymentTransactionCollection(
        IRepositorySession repositorySession,
        IUseCaseRequestPayment requestPayment,
        IUseCaseGetPaymentsByOrder getPaymentsByOrder,
        IUseCaseCheckPaymentStatus checkPaymentStatus)
        : base(new UseCaseGetOdata<PaymentTransaction>(), new UseCaseGet<PaymentTransaction>(), repositorySession)
    {
        _requestPayment = requestPayment;
        _getPaymentsByOrder = getPaymentsByOrder;
        _checkPaymentStatus = checkPaymentStatus;
    }

    public OperationResult<PaymentTransactionDto> Request(RequestPaymentDto command)
    {
        return _requestPayment.Execute(command);
    }

    public OperationResult<IEnumerable<PaymentTransactionDto>> GetByOrder(long orderId)
    {
        return _getPaymentsByOrder.Execute(orderId);
    }

    public OperationResult<PaymentTransactionDto> CheckStatus(long paymentTransactionId)
    {
        return _checkPaymentStatus.Execute(paymentTransactionId);
    }
}
