using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Aplication.Transactions.Utils;
using Payment.Domain.Entities;
using Payment.Domain.Enums;

namespace Payment.Aplication.Transactions.UseCases;

public class RequestPayment : IUseCaseRequestPayment
{
    private readonly IRepositorySession _repositorySession;
    private readonly IPaymentProcessor _paymentProcessor;

    public RequestPayment(IRepositorySession repositorySession, IPaymentProcessor paymentProcessor)
    {
        _repositorySession = repositorySession;
        _paymentProcessor = paymentProcessor;
    }

    public OperationResult<PaymentTransactionDto> Execute(RequestPaymentDto command)
    {
        if (command is null)
        {
            return OperationResult<PaymentTransactionDto>.UnprocessableEntity(new MensagemErro("Pagamento", "Envie os dados da solicitacao de pagamento."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var existingTransaction = repositoryQuery.Query<PaymentTransaction>(payment =>
                payment.OrderId == command.OrderId &&
                (payment.Status == PaymentStatus.Requested || payment.Status == PaymentStatus.Approved))
                .FirstOrDefault();

            if (existingTransaction is not null)
            {
                return OperationResult<PaymentTransactionDto>.UnprocessableEntity(
                    new MensagemErro("Pagamento", "Ja existe uma cobranca ativa para o pedido informado."));
            }

            var transaction = new PaymentTransaction(command.OrderId, command.Amount, command.Method);
            if (!transaction.IsValid)
            {
                return OperationResult<PaymentTransactionDto>.UnprocessableEntity(transaction.Errors);
            }

            var processorResult = _paymentProcessor.RequestPayment(transaction);
            transaction.AttachGatewayId(processorResult.GatewayTransactionId);
            if (!transaction.IsValid)
            {
                return OperationResult<PaymentTransactionDto>.UnprocessableEntity(transaction.Errors);
            }

            var repository = _repositorySession.GetRepository();
            repository.Include(transaction);
            repository.Flush().GetAwaiter().GetResult();

            return OperationResult<PaymentTransactionDto>.Created(transaction.ToDto());
        }
        catch (Exception ex)
        {
            return OperationResult<PaymentTransactionDto>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
