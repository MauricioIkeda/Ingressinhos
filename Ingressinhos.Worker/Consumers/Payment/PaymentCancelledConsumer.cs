using Generic.Messaging.Contracts.Payments;
using Ingressinhos.Application.Sales.Interfaces;

namespace Ingressinhos.Worker.Consumers.Payment;

public class PaymentCancelledConsumer
{
    private readonly ILogger<PaymentCancelledConsumer> _logger;
    private readonly IUseCaseCancelOrderPayment _cancelOrderPayment;

    public PaymentCancelledConsumer(
        ILogger<PaymentCancelledConsumer> logger,
        IUseCaseCancelOrderPayment cancelOrderPayment)
    {
        _logger = logger;
        _cancelOrderPayment = cancelOrderPayment;
    }

    public bool Consume(PaymentCancelledIntegrationEvent message)
    {
        _logger.LogInformation(
            "Processando evento PaymentCancelled para o pedido {orderId}, transacao {paymentTransactionId}. Motivo: {reason}",
            message.OrderId,
            message.PaymentTransactionId,
            message.Reason);

        var cancelResult = _cancelOrderPayment.Execute(message.OrderId);
        if (!cancelResult.Success)
        {
            _logger.LogWarning(
                "Nao foi possivel cancelar o pedido {orderId} apos cancelamento do pagamento: {error}",
                message.OrderId,
                cancelResult.Errors.FirstOrDefault()?.Mensagem ?? "erro nao informado");
            return false;
        }

        _logger.LogInformation("Pedido {orderId} cancelado com sucesso apos cancelamento do pagamento.", message.OrderId);
        return true;
    }
}
