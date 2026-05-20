namespace Generic.Messaging.Contracts.Payments;

// Evento de integracao para outro contexto reagir ao pagamento cancelado ou recusado.
public sealed record PaymentCancelledIntegrationEvent(long PaymentTransactionId,
    long OrderId,
    decimal Amount,
    string Method,
    string Reason,
    DateTime CancelledAtUtc);
