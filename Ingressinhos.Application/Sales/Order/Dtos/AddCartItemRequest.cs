namespace Ingressinhos.Application.Sales.Dtos;

public sealed record AddCartItemRequest(
    long ClientId,
    long TicketId,
    int Quantity,
    long? SeatId);
