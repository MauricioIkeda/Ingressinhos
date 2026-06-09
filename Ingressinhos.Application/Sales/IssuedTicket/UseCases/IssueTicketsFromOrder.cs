using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Entities;
using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Application.Sales.UseCases;

public class IssueTicketsFromOrder : IUseCaseIssueTicketsFromOrder
{
    private readonly IRepositorySession _repositorySession;

    public IssueTicketsFromOrder(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(long orderId)
    {
        if (orderId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Pedido", "Deve ser informado o identificador do pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var order = repositoryQuery.Return<Order>(orderId);

            if (order is null)
            {
                return OperationResult.NotFound(new MensagemErro("Pedido", "Pedido nao encontrado."));
            }

            if (order.Status != OrderStatus.Paid)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("Pedido", "Somente pedidos pagos podem gerar ingressos."));
            }

            var client = repositoryQuery.Return<Client>(order.ClientId);
            if (client is null)
            {
                return OperationResult.NotFound(new MensagemErro("Cliente", "Cliente do pedido nao encontrado."));
            }

            var repository = _repositorySession.GetRepository();
            var utcNow = DateTime.UtcNow;
            var createdAnyTicket = false;

            foreach (var item in order.Items)
            {
                // Idempotencia: calcula somente quantos ingressos ainda faltam para este item.
                var existingTickets = repositoryQuery.Count<IssuedTicket>(ticket => ticket.OrderItemId == item.Id);
                var missingTickets = item.Quantity - existingTickets;
                if (missingTickets <= 0)
                {
                    continue;
                }

                var ticket = repositoryQuery.Return<Ticket>(item.TicketId);
                if (ticket is null)
                {
                    return OperationResult.NotFound(new MensagemErro("Ingresso", $"Nao foi possivel localizar o ingresso do item {item.Id}."));
                }

                for (var i = 0; i < missingTickets; i++)
                {
                    // Cada ingresso emitido recebe um codigo proprio para acesso/check-in futuro.
                    var issuedTicket = new IssuedTicket(item.Id, client.Id, ticket.EventId, GenerateAccessCode(), item.SeatCode, item.Category)
                    {
                        CreatedAt = utcNow,
                        UpdatedAt = utcNow
                    };

                    if (!issuedTicket.IsValid)
                    {
                        return issuedTicket.ToUnprocessableEntityResult();
                    }

                    repository.Include(issuedTicket);
                    createdAnyTicket = true;
                }
            }

            if (!createdAnyTicket)
            {
                return OperationResult.Ok();
            }

            repository.Flush();
            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

    private static string GenerateAccessCode()
    {
        return Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
    }
}
