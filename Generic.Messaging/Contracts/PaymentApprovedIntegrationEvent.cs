namespace Generic.Messaging.Contracts;

// Evento de integracao minimo para outro contexto reagir ao pagamento aprovado.
public sealed class PaymentApprovedIntegrationEvent
{
    public long PaymentTransactionId { get; init; }
    public long OrderId { get; init; }
    public decimal Amount { get; init; }
    public string Method { get; init; } = string.Empty;
    public DateTime ApprovedAtUtc { get; init; }
}
