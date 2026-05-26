using MongoDB.Bson.Serialization.Attributes;

namespace Ingressinhos.Infrastructure.ReadModels.TicketReadModel;

public class ClientTicketDocument
{
    [BsonId] // Esses atributos são o mapeamento do MongoDB
    public long Id { get; set; }

    [BsonElement("issuedTicketId")] // E esses só para deixar com viadagem de pascal case, não tem muita vantagem
    public long IssuedTicketId { get; set; }

    [BsonElement("accessCode")]
    public string AccessCode { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("issuedAtUtc")]
    public DateTime IssuedAtUtc { get; set; }

    [BsonElement("checkedInAtUtc")]
    public DateTime? CheckedInAtUtc { get; set; }

    [BsonElement("cancelledAtUtc")]
    public DateTime? CancelledAtUtc { get; set; }

    [BsonElement("paidAtUtc")]
    public DateTime? PaidAtUtc { get; set; }

    [BsonElement("clientId")]
    public long ClientId { get; set; }

    [BsonElement("clientUserId")]
    public string ClientUserId { get; set; } = string.Empty;

    [BsonElement("orderId")]
    public long OrderId { get; set; }

    [BsonElement("orderItemId")]
    public long OrderItemId { get; set; }

    [BsonElement("ticketName")]
    public string TicketName { get; set; } = string.Empty;

    [BsonElement("seatCode")]
    public string SeatCode { get; set; }

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("eventId")]
    public long EventId { get; set; }

    [BsonElement("eventName")]
    public string EventName { get; set; } = string.Empty;

    [BsonElement("eventStartTimeUtc")]
    public DateTime EventStartTimeUtc { get; set; }

    [BsonElement("eventEndTimeUtc")]
    public DateTime EventEndTimeUtc { get; set; }

    [BsonElement("eventImageUrl")]
    public string EventImageUrl { get; set; } = string.Empty;

    [BsonElement("locationId")]
    public long LocationId { get; set; }

    [BsonElement("locationName")]
    public string LocationName { get; set; } = string.Empty;

    [BsonElement("projectedAtUtc")]
    public DateTime ProjectedAtUtc { get; set; }
}
