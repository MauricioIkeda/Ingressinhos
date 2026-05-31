using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Ingressinhos.Application.Sales.TicketReadModel.Dtos;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Models;

namespace Ingressinhos.Application.Sales.TicketReadModel.UseCases;

public class GetMyClientTickets : IUseCaseGetMyClientTickets
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IClientTicketReadModelQuery _query;

    public GetMyClientTickets(ICurrentUserContext currentUserContext, IClientTicketReadModelQuery query)
    {
        _currentUserContext = currentUserContext;
        _query = query;
    }

    public OperationResult<List<ClientTicketViewDto>> Execute(int skip = 0, int top = 50)
    {
        if (!_currentUserContext.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserContext.UserId))
        {
            return OperationResult<List<ClientTicketViewDto>>.Unauthorized(new MensagemErro("Usuario", "Usuario nao autenticado."));
        }

        if (skip < 0)
        {
            return OperationResult<List<ClientTicketViewDto>>.UnprocessableEntity(new MensagemErro("Skip", "O valor de skip nao pode ser negativo."));
        }

        if (top <= 0 || top > 100)
        {
            return OperationResult<List<ClientTicketViewDto>>.UnprocessableEntity(new MensagemErro("Top", "O valor de top deve estar entre 1 e 100."));
        }

        try
        {
            var tickets = _query.GetByClientUserId(_currentUserContext.UserId)
                .Select(ToDto)
                .OrderByDescending(ticket => ticket.IssuedAtUtc)
                .ThenByDescending(ticket => ticket.IssuedTicketId)
                .Skip(skip)
                .Take(top)
                .ToList();

            return OperationResult<List<ClientTicketViewDto>>.Ok(tickets);
        }
        catch (Exception ex)
        {
            return OperationResult<List<ClientTicketViewDto>>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

    private static ClientTicketViewDto ToDto(ClientTicketReadModelEntry ticket)
    {
        return new ClientTicketViewDto
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
