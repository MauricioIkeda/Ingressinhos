namespace Generic.Messaging.Contracts;

public static class MessageQueues // Classe estatica pq eh fixo
{
    public const string PaymentApproved = "payment-approved";
    public const string PaymentCancelled = "payment-cancelled";
    public const string TicketReadModelSync = "ticket-read-model-sync";
}
