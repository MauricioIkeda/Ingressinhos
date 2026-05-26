using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Models;
using Ingressinhos.Domain.Sales.Entities;

namespace Ingressinhos.Application.Sales.TicketReadModel.UseCases;

public class ProjectClientTicketsFromOrder : IUseCaseProjectClientTicketsFromOrder
{
    private readonly IRepositorySession _repositorySession;
    private readonly IClientTicketReadModelWriter _writer;
    private readonly ClientTicketReadModelBuilder _builder;

    public ProjectClientTicketsFromOrder(IRepositorySession repositorySession, IClientTicketReadModelWriter writer, ClientTicketReadModelBuilder builder)
    {
        _repositorySession = repositorySession;
        _writer = writer;
        _builder = builder;
    }

    public OperationResult Execute(long orderId)
    {
        if (orderId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Order", "Deve ser informado o identificador do pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var order = repositoryQuery.Return<Order>(orderId);
            if (order is null)
            {
                return OperationResult.NotFound(new MensagemErro("Order", "Pedido nao encontrado."));
            }

            var orderItemIds = order.Items.Select(item => item.Id).ToArray();
            if (orderItemIds.Length == 0)
            {
                return OperationResult.Ok();
            }

            var issuedTicketIds = repositoryQuery.Query<IssuedTicket>(ticket => orderItemIds.Contains(ticket.OrderItemId))
                .OrderBy(ticket => ticket.Id)
                .Select(ticket => ticket.Id)
                .ToList();

            return ProjectTickets(issuedTicketIds, repositoryQuery);
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

    private OperationResult ProjectTickets(IReadOnlyCollection<long> issuedTicketIds, IRepositoryQuery repositoryQuery)
    {
        var entries = new List<ClientTicketReadModelEntry>();
        foreach (var issuedTicketId in issuedTicketIds)
        {
            var buildResult = _builder.Build(issuedTicketId, repositoryQuery);
            if (!buildResult.Success)
            {
                return OperationResult.Fail(buildResult.Errors);
            }

            entries.Add(buildResult.Data);
        }

        if (entries.Count > 0)
        {
            _writer.UpsertMany(entries);
        }

        return OperationResult.Ok();
    }
}
