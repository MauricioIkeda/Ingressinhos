using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class TicketInclude : IUseCaseCommand<TicketDto>
{
    private readonly IRepositorySession _repositorySession;

    public TicketInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(TicketDto ticket)
    {
        if (ticket is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Ticket", "Deve ser informado o ingresso."));
        }

        try
        {
            var utcNow = DateTime.UtcNow;
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();

            var littleEvent = repositoryQuery.Return<Event>(ticket.EventId);
            if (littleEvent == null)
            {
                return OperationResult.NotFound(new MensagemErro("EventId", "Evento nao encontrado."));
            }

            Seller seller = repositoryQuery.Return<Seller>(ticket.SellerId);
            if (seller == null)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("SellerId", "O vendedor deve ser informado."));
            }

            if (littleEvent.SellerId != seller.Id)
            {
                return OperationResult.Forbidden(new MensagemErro("SellerId", "O vendedor do ingresso deve ser dono do evento."));
            }
            
            LocationDomain location = repositoryQuery.Return<LocationDomain>(littleEvent.LocationId);
            if (location is null)
            {
                return OperationResult.NotFound(new MensagemErro("LocationId", "Local do evento nao encontrado."));
            }

            var ticketEntity = new Ticket(
                ticket.EventId,
                ticket.SellerId,
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

            if (!ticket.IsActive)
            {
                ticketEntity.Disable();
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
