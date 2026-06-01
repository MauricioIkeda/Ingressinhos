using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class EventDelete
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public EventDelete(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(long eventId)
    {
        if (eventId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do evento."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var eventEntity = repositoryQuery.Return<Event>(eventId);
            if (eventEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Evento nao encontrado."));
            }

            var seller = CurrentUserEntityResolver.ResolveSeller(_currentUserContext, repositoryQuery, eventEntity.SellerId);
            if (seller is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.NotFound(new MensagemErro("Seller", "Vendedor do evento nao foi encontrado."))
                    : OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua loja."));
            }

            if (eventEntity.SellerId != seller.Id)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.Forbidden(new MensagemErro("Evento", "O evento informado nao pertence ao vendedor selecionado."))
                    : OperationResult.Forbidden(new MensagemErro("Evento", "Voce so pode excluir eventos da sua propria loja."));
            }

            var ticketIds = repositoryQuery.Query<Ticket>(ticket => ticket.EventId == eventEntity.Id)
                .Select(ticket => ticket.Id)
                .ToList();

            if (ticketIds.Count > 0 && repositoryQuery.Query<OrderItem>(item => ticketIds.Contains(item.TicketId)).Any())
            {
                return OperationResult.UnprocessableEntity(new MensagemErro(
                    "Evento",
                    "Nao e possivel excluir um evento que ja possui carrinhos, pedidos ou ingressos vinculados."));
            }

            var repository = _repositorySession.GetRepository();
            repository.Delete(eventEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
