using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Entities;
using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Application.Catalog.UseCases;

public class EventGetSeats
{
    private readonly IReadRepositoryQuery _readRepositoryQuery;

    public EventGetSeats(IReadRepositoryQuery readRepositoryQuery)
    {
        _readRepositoryQuery = readRepositoryQuery;
    }

    public OperationResult<List<EventSeatAvailabilityDto>> Execute(long eventId)
    {
        if (eventId <= 0)
        {
            return OperationResult<List<EventSeatAvailabilityDto>>.UnprocessableEntity(new MensagemErro("EventId", "Deve ser informado o evento da consulta."));
        }

        try
        {
            var eventEntity = _readRepositoryQuery.Return<Event>(eventId);
            if (eventEntity is null)
            {
                return OperationResult<List<EventSeatAvailabilityDto>>.NotFound(new MensagemErro("EventId", "Evento nao encontrado."));
            }

            if (!eventEntity.HasSeats)
            {
                return OperationResult<List<EventSeatAvailabilityDto>>.UnprocessableEntity(new MensagemErro("EventId", "Este evento nao usa assento marcado."));
            }

            var seats = _readRepositoryQuery.Query<Seat>()
                .Where(seat => seat.LocationId == eventEntity.LocationId) // Pega assentos do mesmo local
                .OrderBy(seat => seat.Code)
                .ToList();

            var seatIds = seats.Select(seat => seat.Id).ToArray(); // lista dos ids

            var reservations = _readRepositoryQuery.Query<SeatReservation>() // Pega reservas ativas para os assentos do evento
                .Where(reservation =>
                    reservation.EventId == eventId &&
                    seatIds.Contains(reservation.SeatId) &&
                    (reservation.Status == SeatReservationStatus.Reserved || reservation.Status == SeatReservationStatus.Occupied))
                .ToList()
                .ToDictionary(reservation => reservation.SeatId);

            var result = seats.Select(seat => // Para cada assento, verifica se tem reserva ativa e resolve o status
            {
                reservations.TryGetValue(seat.Id, out var reservation); // Tenta pegar a reserva ativa para o assento, se tiver
                return new EventSeatAvailabilityDto //Retorna o DTO
                {
                    SeatId = seat.Id,
                    Code = seat.Code,
                    Category = seat.Category,
                    Status = SeatReservationRulesHelper.ResolveEventSeatStatus(seat, reservation)
                };
            }).ToList();

            return OperationResult<List<EventSeatAvailabilityDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            return OperationResult<List<EventSeatAvailabilityDto>>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
