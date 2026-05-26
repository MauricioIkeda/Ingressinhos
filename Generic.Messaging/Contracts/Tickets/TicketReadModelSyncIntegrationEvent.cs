namespace Generic.Messaging.Contracts.Tickets;

public sealed record TicketReadModelSyncIntegrationEvent( 
    TicketReadModelSyncKind SyncKind, // a entidade mudada
    long ReferenceId, // o id da entidade
    DateTime RequestedAtUtc);
