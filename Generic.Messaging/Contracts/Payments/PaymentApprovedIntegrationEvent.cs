namespace Generic.Messaging.Contracts.Payments;

// Evento de integracao minimo para outro contexto reagir ao pagamento aprovado.
public sealed record PaymentApprovedIntegrationEvent(
    long PaymentTransactionId,
    long OrderId,
    decimal Amount,
    string Method,
    DateTime ApprovedAtUtc);
