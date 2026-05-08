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
            var existingApprovedTransaction = repositoryQuery.Query<PaymentTransaction>(payment =>
                payment.OrderId == command.OrderId &&
                payment.Status == PaymentStatus.Approved)
                .FirstOrDefault();

            if (existingApprovedTransaction is not null)
            {
                return OperationResult<PaymentTransactionDto>.UnprocessableEntity(
                    new MensagemErro("Pagamento", "Ja existe um pagamento aprovado para o pedido informado."));
            }

            var previousTransactions = repositoryQuery.Query<PaymentTransaction>(payment =>
                    payment.OrderId == command.OrderId &&
                    payment.Status != PaymentStatus.Approved &&
                    payment.Status != PaymentStatus.Cancelled)
                .OrderByDescending(payment => payment.RequestedAt)
                .ToList();

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
            foreach (var previousTransaction in previousTransactions)
            {
                previousTransaction.Cancel();
                if (!previousTransaction.IsValid)
                {
                    return OperationResult<PaymentTransactionDto>.UnprocessableEntity(previousTransaction.Errors);
                }

                repository.Upsert(previousTransaction);
            }

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
