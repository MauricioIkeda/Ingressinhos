using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Refunds.Dtos;
using Payment.Aplication.Refunds.Interfaces;
using Payment.Aplication.Refunds.Utils;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Domain.Entities;
using Payment.Domain.Enums;

namespace Payment.Aplication.Refunds.UseCases;

public class CheckRefundStatus : IUseCaseCheckRefundStatus
{
    private readonly IRepositorySession _repositorySession;
    private readonly IPaymentProcessor _paymentProcessor;

    public CheckRefundStatus(IRepositorySession repositorySession, IPaymentProcessor paymentProcessor)
    {
        _repositorySession = repositorySession;
        _paymentProcessor = paymentProcessor;
    }

    public OperationResult<RefundDto> Execute(long refundId)
    {
        if (refundId <= 0)
        {
            return OperationResult<RefundDto>.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do reembolso."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var refund = repositoryQuery.Return<Refund>(refundId);
            if (refund is null)
            {
                return OperationResult<RefundDto>.NotFound(new MensagemErro("Reembolso", "Reembolso nao encontrado."));
            }

            if (refund.Status == RefundStatus.Requested)
            {
                var processorResult = _paymentProcessor.ResolveRefund(refund);

                if (processorResult.Status == RefundStatus.Completed)
                {
                    refund.Complete();
                }
                else if (processorResult.Status == RefundStatus.Rejected)
                {
                    refund.Reject();
                }

                if (!refund.IsValid)
                {
                    return OperationResult<RefundDto>.UnprocessableEntity(refund.Errors);
                }

                var repository = _repositorySession.GetRepository();
                repository.Upsert(refund);
                repository.Flush();
            }

            return OperationResult<RefundDto>.Ok(refund.ToDto());
        }
        catch (Exception ex)
        {
            return OperationResult<RefundDto>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
