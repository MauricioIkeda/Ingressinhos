using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using LocationDomain = Ingressinhos.Domain.Catalog.Entities.Location;

namespace Ingressinhos.Application.Catalog.UseCases;

public class TicketInclude : IUseCaseCommand<TicketDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public TicketInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(TicketDto ticket)
    {
        Messages.Clear();

        if (ticket is null)
        {
            Messages.Add("Deve ser informado o ingresso", error: true);
            return false;
        }

        try
        {
            var utcNow = DateTime.UtcNow;
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();

            var littleEvent = repositoryQuery.Return<Event>(ticket.EventId);
            if (littleEvent == null)
            {
                Messages.Add("Evento nao encontrado", error: true);
                return false;
            }

            Seller seller = repositoryQuery.Return<Seller>(ticket.SellerId);
            if (seller == null)
            {
                Messages.Add("O vendedor deve ser informado!", error: true);
                return false;
            }

            if (littleEvent.SellerId != seller.Id)
            {
                Messages.Add("O vendedor do ingresso deve ser dono do evento", error: true);
                return false;
            }
            
            LocationDomain location = repositoryQuery.Return<LocationDomain>(littleEvent.LocationId);
            if (location is null)
            {
                Messages.Add("Local do evento nao encontrado", error: true);
                return false;
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
            return true;
        }
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }
}
