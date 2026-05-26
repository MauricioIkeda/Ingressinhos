namespace Ingressinhos.Infrastructure.Options;

public class ClientTicketMongoOptions // MongoDB Conexão
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string Database { get; set; } = "IngressinhosReadDb";
    public string TicketCollection { get; set; } = "clientTickets";
}
