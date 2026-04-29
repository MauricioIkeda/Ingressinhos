using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Application.Catalog.UseCases;

public class TicketUpdate : IUseCaseCommand<TicketDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public TicketUpdate(IRepositorySession repositorySession)
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

        if (ticket.TicketId <= 0)
        {
            Messages.Add("Deve ser informado o identificador do ingresso", error: true);
            return false;
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var ticketEntity = repositoryQuery.Return<Ticket>(ticket.TicketId);

            if (ticketEntity is null)
            {
                Messages.Add("Ingresso nao encontrado", error: true);
                return false;
            }

            if (ticket.BasePrice != ticketEntity.BasePrice.Value ||
                ticket.PremiumPrice != ticketEntity.PremiumPrice?.Value ||
                ticket.VipPrice != ticketEntity.VIPPrice?.Value)
            {
                ticketEntity.ChangePrices(ticket.BasePrice, ticket.PremiumPrice, ticket.VipPrice);
            }

            if (ticket.IsActive && ticketEntity.Status == CatalogTicketStatus.Inactive)
            {
                ticketEntity.Enable();
            }

            if (!ticket.IsActive && ticketEntity.Status != CatalogTicketStatus.Inactive)
            {
                ticketEntity.Disable();
            }

            ticketEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(ticketEntity);
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
