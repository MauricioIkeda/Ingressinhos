namespace Generic.Messaging.Contracts.Tickets;

public enum TicketReadModelSyncKind // Enum criado para identificar o tipo da sincronizacao
{
    OrderTickets = 1,
    IssuedTicket = 2,
    Event = 3,
    Location = 4
}
