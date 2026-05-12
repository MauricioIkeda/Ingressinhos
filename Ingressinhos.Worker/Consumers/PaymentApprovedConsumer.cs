using Generic.Messaging.Contracts;
using Ingressinhos.Application.Sales.Interfaces;

namespace Ingressinhos.Worker.Consumers;

public class PaymentApprovedConsumer
{
    private readonly ILogger<PaymentApprovedConsumer> _logger;
    private readonly IUseCaseConfirmOrderPayment _confirmOrderPayment;
    private readonly IUseCaseIssueTicketsFromOrder _issueTicketsFromOrder;

    public PaymentApprovedConsumer(
        ILogger<PaymentApprovedConsumer> logger,
        IUseCaseConfirmOrderPayment confirmOrderPayment,
        IUseCaseIssueTicketsFromOrder issueTicketsFromOrder)
    {
        _logger = logger;
        _confirmOrderPayment = confirmOrderPayment;
        _issueTicketsFromOrder = issueTicketsFromOrder;
    }

    public bool Consume(PaymentApprovedIntegrationEvent message)
    {
        _logger.LogInformation(
            "Processando evento PaymentApproved para o pedido {orderId} e transacao {paymentTransactionId}.",
            message.OrderId,
            message.PaymentTransactionId);

        // Primeiro refletimos o fato no pedido.
        var confirmResult = _confirmOrderPayment.Execute(message.OrderId);
        if (!confirmResult.Success)
        {
            _logger.LogWarning(
                "Nao foi possivel confirmar o pagamento do pedido {orderId}: {error}",
                message.OrderId,
                confirmResult.Errors.FirstOrDefault()?.Mensagem ?? "erro nao informado");
            return false;
        }

        // Depois emitimos os ingressos derivados daquele pedido pago.
        var issueResult = _issueTicketsFromOrder.Execute(message.OrderId);
        if (!issueResult.Success)
        {
            _logger.LogWarning(
                "Nao foi possivel emitir os ingressos do pedido {orderId}: {error}",
                message.OrderId,
                issueResult.Errors.FirstOrDefault()?.Mensagem ?? "erro nao informado");
            return false;
        }

        _logger.LogInformation("Pedido {orderId} processado com sucesso apos aprovacao do pagamento.", message.OrderId);
        return true;
    }
}
