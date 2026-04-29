using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Catalog.Enums;

namespace Ingressinhos.Application.Catalog.UseCases;

public class TicketUpdate : IUseCaseCommand<TicketDto>
{
    private readonly IRepositorySession _repositorySession;

    public TicketUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(TicketDto ticket)
    {
        if (ticket is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Ticket", "Deve ser informado o ingresso."));
        }

        if (ticket.TicketId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do ingresso."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var ticketEntity = repositoryQuery.Return<Ticket>(ticket.TicketId);

            if (ticketEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Ingresso nao encontrado."));
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
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
