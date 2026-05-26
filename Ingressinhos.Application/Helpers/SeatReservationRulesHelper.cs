using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;
using Ingressinhos.Domain.Sales.Entities;
using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Application.Helpers;

internal static class SeatReservationRulesHelper
{
    public const string ActiveSeatReservationIndexName = "IX_SeatReservations_EventId_SeatId_Active"; // chave para evitar concorrência na execução

    public static SeatReservation GetActiveReservation(IRepositoryQuery repositoryQuery, long eventId, long seatId)
    {
        return repositoryQuery.Query<SeatReservation>(reservation => // pega aqui aqueles com reserva no banco
                reservation.EventId == eventId &&
                reservation.SeatId == seatId &&
                (reservation.Status == SeatReservationStatus.Reserved || reservation.Status == SeatReservationStatus.Occupied))
            .FirstOrDefault();
    }

    public static SeatReservation GetActiveReservationForOrderItem(IRepositoryQuery repositoryQuery, long orderId, long orderItemId)
    {
        // Usado na confirmacao/cancelamento do pagamento para encontrar a reserva
        // criada no fechamento daquele item especifico do pedido.
        return repositoryQuery.Query<SeatReservation>(reservation =>
                reservation.OrderId == orderId &&
                reservation.OrderItemId == orderItemId &&
                (reservation.Status == SeatReservationStatus.Reserved || reservation.Status == SeatReservationStatus.Occupied))
            .FirstOrDefault();
    }

    public static SeatStatus ResolveEventSeatStatus(Seat seat, SeatReservation reservation)
    {
        // Bloqueio fisico do assento vence qualquer status de venda do evento.
        if (seat.Status == SeatStatus.Blocked)
        {
            return SeatStatus.Blocked;
        }

        // "Gambiarra" para não precisar refatorar todo o codigo, só conversão de um enum para outro.
        return reservation?.Status switch
        {
            SeatReservationStatus.Reserved => SeatStatus.Reserved,
            SeatReservationStatus.Occupied => SeatStatus.Occupied,
            _ => SeatStatus.Available
        };
    }

    public static OperationResult UnavailableSeatResult(string seatCode)
    {
        // Mensagem padrao para conflito de reserva ou assento fisicamente bloqueado.
        return OperationResult.UnprocessableEntity(new MensagemErro("SeatId", $"O assento {seatCode} nao esta disponivel."));
    }

    public static bool IsSeatReservationConflict(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException) // Analisa todas as exeções até não serem nulas
        {
            if (current.Message.Contains(ActiveSeatReservationIndexName, StringComparison.OrdinalIgnoreCase)) // se a mensagem de erro da exceção contiver o nome do índice único, é um conflito de reserva
            {
                return true;
            }
        }

        return false;
    }
}
