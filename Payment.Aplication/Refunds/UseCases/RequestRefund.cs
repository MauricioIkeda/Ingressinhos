using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Refunds.Dtos;
using Payment.Aplication.Refunds.Interfaces;
using Payment.Aplication.Refunds.Utils;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Domain.Entities;
using Payment.Domain.Enums;

namespace Payment.Aplication.Refunds.UseCases;

public class RequestRefund : IUseCaseRequestRefund
{
    private readonly IRepositorySession _repositorySession;
    private readonly IPaymentProcessor _paymentProcessor;

    public RequestRefund(IRepositorySession repositorySession, IPaymentProcessor paymentProcessor)
    {
        _repositorySession = repositorySession;
        _paymentProcessor = paymentProcessor;
    }

    public OperationResult<RefundDto> Execute(RequestRefundDto command)
    {
        if (command is null)
        {
            return OperationResult<RefundDto>.UnprocessableEntity(new MensagemErro("Reembolso", "Envie os dados da solicitacao de reembolso."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var paymentTransaction = repositoryQuery.Return<PaymentTransaction>(command.PaymentTransactionId);

            if (paymentTransaction is null)
            {
                return OperationResult<RefundDto>.NotFound(new MensagemErro("Pagamento", "Transacao de pagamento nao encontrada."));
            }

            if (paymentTransaction.Status != PaymentStatus.Approved)
            {
                return OperationResult<RefundDto>.UnprocessableEntity(new MensagemErro("Pagamento", "Somente pagamentos aprovados podem receber reembolso."));
            }

            var reservedRefundAmount = repositoryQuery.Query<Refund>(refund =>
                    refund.PaymentTransactionId == command.PaymentTransactionId &&
                    refund.Status != RefundStatus.Rejected)
                .ToList()
                .Sum(refund => refund.Amount.Value);

            var availableAmount = paymentTransaction.Amount.Value - reservedRefundAmount;
            if (command.Amount > availableAmount)
            {
                return OperationResult<RefundDto>.UnprocessableEntity(
                    new MensagemErro("Reembolso", "O valor solicitado ultrapassa o saldo disponivel para reembolso."));
            }

            var refundEntity = new Refund(command.PaymentTransactionId, command.Amount, command.Reason);
            if (!refundEntity.IsValid)
            {
                return OperationResult<RefundDto>.UnprocessableEntity(refundEntity.Errors);
            }

            var processorResult = _paymentProcessor.RequestRefund(refundEntity);
            if (string.IsNullOrWhiteSpace(processorResult.GatewayRefundId))
            {
                return OperationResult<RefundDto>.UnprocessableEntity(
                    new MensagemErro("Gateway", "Nao foi possivel iniciar o reembolso no processador de pagamento."));
            }

            var repository = _repositorySession.GetRepository();
            repository.Include(refundEntity);
            repository.Flush().GetAwaiter().GetResult();

            return OperationResult<RefundDto>.Created(refundEntity.ToDto());
        }
        catch (Exception ex)
        {
            return OperationResult<RefundDto>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
