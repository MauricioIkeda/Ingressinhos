using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Refunds.Dtos;
using Payment.Aplication.Refunds.Interfaces;
using Payment.Aplication.Refunds.Utils;
using Payment.Domain.Entities;

namespace Payment.Aplication.Refunds.UseCases;

public class GetRefundsByPaymentTransaction : IUseCaseGetRefundsByPaymentTransaction
{
    private readonly IRepositorySession _repositorySession;

    public GetRefundsByPaymentTransaction(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult<IEnumerable<RefundDto>> Execute(long paymentTransactionId)
    {
        if (paymentTransactionId <= 0)
        {
            return OperationResult<IEnumerable<RefundDto>>.UnprocessableEntity(new MensagemErro("Pagamento", "Deve ser informada a transacao de pagamento."));
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var refunds = repositoryQuery.Query<Refund>(refund => refund.PaymentTransactionId == paymentTransactionId)
            .OrderByDescending(refund => refund.RequestedAt)
            .ToList()
            .Select(refund => refund.ToDto());

        return OperationResult<IEnumerable<RefundDto>>.Ok(refunds);
    }
}
