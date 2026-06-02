using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Ingressinhos.Application.Sales.TicketReadModel.Dtos;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

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

    public OperationResult<List<TOutput>> Execute<TOutput>(Func<IQueryable<ClientTicketViewDto>, IQueryable<TOutput>> query)
    {
        if (query is null)
        {
            return OperationResult<List<TOutput>>.UnprocessableEntity(new MensagemErro("Filtro", "Transformacao da consulta deve ser informada."));
        }

        if (!_currentUserContext.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserContext.UserId))
        {
            return OperationResult<List<TOutput>>.Unauthorized(new MensagemErro("Usuario", "Usuario nao autenticado."));
        }

        try
        {
            var userId = _currentUserContext.UserId.Trim();
            var tickets = _query.Get(ticketsQuery => // Consulta com getodata, mas somente para o me
            {
                var userTickets = ticketsQuery
                    .Where(ticket => ticket.ClientUserId == userId) // garantindo que só mostre aqueles do usuário
                    .OrderByDescending(ticket => ticket.IssuedAtUtc)
                    .ThenByDescending(ticket => ticket.IssuedTicketId);

                return query(userTickets); // aplicando a query 
            });

            return OperationResult<List<TOutput>>.Ok(tickets.ToList());
        }
        catch (Exception ex)
        {
            return OperationResult<List<TOutput>>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
