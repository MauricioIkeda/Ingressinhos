using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class TicketInclude : IUseCaseCommand<TicketDto>
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public TicketInclude(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(TicketDto ticket)
    {
        if (ticket is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Ingresso", "Envie os dados do ingresso."));
        }

        if (_currentUserContext.Role == "Admin" && ticket.SellerId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("SellerId", "Deve ser informado o identificador do vendedor."));
        }

        try
        {
            var utcNow = DateTime.UtcNow;
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();
            var seller = CurrentUserEntityResolver.ResolveSeller(_currentUserContext, repositoryQuery, ticket.SellerId);
            if (seller is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.NotFound(new MensagemErro("Id", "Vendedor nao encontrado."))
                    : OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua loja."));
            }

            if (!seller.Active)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Seller", "Nao e possivel cadastrar ingresso para um vendedor desativado."));
            }

            var littleEvent = repositoryQuery.Return<Event>(ticket.EventId);
            if (littleEvent == null)
            {
                return OperationResult.NotFound(new MensagemErro("Evento", "Nao encontramos o evento informado."));
            }

            if (littleEvent.SellerId != seller.Id)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.Forbidden(new MensagemErro("Evento", "O evento informado nao pertence ao vendedor selecionado."))
                    : OperationResult.Forbidden(new MensagemErro("Evento", "Voce so pode cadastrar ingressos em eventos da sua loja."));
            }
            
            LocationDomain location = repositoryQuery.Return<LocationDomain>(littleEvent.LocationId);
            if (location is null)
            {
                return OperationResult.NotFound(new MensagemErro("Local", "Nao encontramos o local deste evento."));
            }

            var ticketEntity = new Ticket(
                ticket.EventId,
                seller.Id,
                ticket.Name,
                ticket.BasePrice,
                ticket.PremiumPrice,
                ticket.VipPrice,
                location.TotalCapacity,
                ticket.SalesStartsAt,
                ticket.SalesEndsAt)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
            if (!ticketEntity.IsValid)
            {
                return ticketEntity.ToUnprocessableEntityResult();
            }

            if (!ticket.IsActive)
            {
                ticketEntity.Disable();
                if (!ticketEntity.IsValid)
                {
                    return ticketEntity.ToUnprocessableEntityResult();
                }
            }

            var repository = _repositorySession.GetRepository();
            repository.Include(ticketEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

}
