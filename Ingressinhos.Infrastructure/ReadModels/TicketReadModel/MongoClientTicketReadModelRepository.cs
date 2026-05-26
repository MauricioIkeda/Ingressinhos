using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Models;
using Ingressinhos.Infrastructure.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ingressinhos.Infrastructure.ReadModels.TicketReadModel;

public class MongoClientTicketReadModelRepository : IClientTicketReadModelWriter, IClientTicketReadModelQuery, IClientTicketReadModelHealthCheck
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<ClientTicketDocument> _collection;
    private readonly object _indexLock = new();
    private bool _indexesEnsured;

    public MongoClientTicketReadModelRepository(ClientTicketMongoOptions options)  // Construtor
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var client = new MongoClient(options.ConnectionString);
        _database = client.GetDatabase(options.Database);
        _collection = _database.GetCollection<ClientTicketDocument>(options.TicketCollection);
    }

    public void Upsert(ClientTicketReadModelEntry ticket) // Inclui ou atualiza
    {
        if (ticket is null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        EnsureIndexes();

        var document = ToDocument(ticket);
        _collection.ReplaceOne(
            item => item.Id == document.Id,
            document,
            new ReplaceOptions { IsUpsert = true });
    }

    public void UpsertMany(IReadOnlyCollection<ClientTicketReadModelEntry> tickets) // Inclui ou atualiza, mas com muitos
    {
        if (tickets is null || tickets.Count == 0)
        {
            return;
        }

        foreach (var ticket in tickets)
        {
            Upsert(ticket);
        }
    }

    public IReadOnlyCollection<ClientTicketReadModelEntry> GetByClientUserId(string clientUserId) // Pegar os tickets
    {
        if (string.IsNullOrWhiteSpace(clientUserId))
        {
            return [];
        }

        EnsureIndexes();

        return _collection
            .Find(ticket => ticket.ClientUserId == clientUserId.Trim())
            .SortBy(ticket => ticket.EventStartTimeUtc)
            .ThenBy(ticket => ticket.IssuedTicketId)
            .ToList()
            .Select(ToEntry)
            .ToList();
    }

    public bool IsAvailable() // conferir se o banco está ON
    {
        try
        {
            _database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void EnsureIndexes() // Garantir que os índices estão criados, para evitar problemas de concorrência e performance. O lock é para evitar que múltiplas threads tentem criar os índices ao mesmo tempo.
    {
        if (_indexesEnsured)
        {
            return;
        }

        lock (_indexLock)
        {
            if (_indexesEnsured)
            {
                return;
            }

            var indexes = new[]
            {
                new CreateIndexModel<ClientTicketDocument>(
                    Builders<ClientTicketDocument>.IndexKeys.Ascending(ticket => ticket.AccessCode),
                    new CreateIndexOptions { Unique = true, Name = "UX_ClientTickets_AccessCode" }),
                new CreateIndexModel<ClientTicketDocument>(
                    Builders<ClientTicketDocument>.IndexKeys.Ascending(ticket => ticket.ClientUserId),
                    new CreateIndexOptions { Name = "IX_ClientTickets_ClientUserId" }),
                new CreateIndexModel<ClientTicketDocument>(
                    Builders<ClientTicketDocument>.IndexKeys.Ascending(ticket => ticket.EventId),
                    new CreateIndexOptions { Name = "IX_ClientTickets_EventId" })
            };

            _collection.Indexes.CreateMany(indexes);
            _indexesEnsured = true;
        }
    }

    private static ClientTicketDocument ToDocument(ClientTicketReadModelEntry ticket) // Conversão do modelo de leitura para o documento do MongoDB
    {
        return new ClientTicketDocument
        {
            Id = ticket.IssuedTicketId,
            IssuedTicketId = ticket.IssuedTicketId,
            AccessCode = ticket.AccessCode,
            Status = ticket.Status,
            IssuedAtUtc = ticket.IssuedAtUtc,
            CheckedInAtUtc = ticket.CheckedInAtUtc,
            CancelledAtUtc = ticket.CancelledAtUtc,
            PaidAtUtc = ticket.PaidAtUtc,
            ClientId = ticket.ClientId,
            ClientUserId = ticket.ClientUserId,
            OrderId = ticket.OrderId,
            OrderItemId = ticket.OrderItemId,
            TicketName = ticket.TicketName,
            SeatCode = ticket.SeatCode,
            Category = ticket.Category,
            EventId = ticket.EventId,
            EventName = ticket.EventName,
            EventStartTimeUtc = ticket.EventStartTimeUtc,
            EventEndTimeUtc = ticket.EventEndTimeUtc,
            EventImageUrl = ticket.EventImageUrl,
            LocationId = ticket.LocationId,
            LocationName = ticket.LocationName,
            ProjectedAtUtc = ticket.ProjectedAtUtc
        };
    }

    private static ClientTicketReadModelEntry ToEntry(ClientTicketDocument ticket) // Converte documento para modelo de leitura
    {
        return new ClientTicketReadModelEntry
        {
            IssuedTicketId = ticket.IssuedTicketId,
            AccessCode = ticket.AccessCode,
            Status = ticket.Status,
            IssuedAtUtc = ticket.IssuedAtUtc,
            CheckedInAtUtc = ticket.CheckedInAtUtc,
            CancelledAtUtc = ticket.CancelledAtUtc,
            PaidAtUtc = ticket.PaidAtUtc,
            ClientId = ticket.ClientId,
            ClientUserId = ticket.ClientUserId,
            OrderId = ticket.OrderId,
            OrderItemId = ticket.OrderItemId,
            TicketName = ticket.TicketName,
            SeatCode = ticket.SeatCode,
            Category = ticket.Category,
            EventId = ticket.EventId,
            EventName = ticket.EventName,
            EventStartTimeUtc = ticket.EventStartTimeUtc,
            EventEndTimeUtc = ticket.EventEndTimeUtc,
            EventImageUrl = ticket.EventImageUrl,
            LocationId = ticket.LocationId,
            LocationName = ticket.LocationName,
            ProjectedAtUtc = ticket.ProjectedAtUtc
        };
    }
}
