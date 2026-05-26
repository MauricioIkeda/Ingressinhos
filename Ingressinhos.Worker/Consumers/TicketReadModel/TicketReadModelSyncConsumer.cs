using Generic.Messaging.Contracts.Tickets;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

namespace Ingressinhos.Worker.Consumers.TicketReadModel;

public class TicketReadModelSyncConsumer
{
    private readonly ILogger<TicketReadModelSyncConsumer> _logger;
    private readonly IUseCaseProjectClientTicketsFromOrder _projectClientTicketsFromOrder;
    private readonly IUseCaseProjectClientTicketFromIssuedTicket _projectClientTicketFromIssuedTicket;
    private readonly IUseCaseRefreshClientTicketsByEvent _refreshClientTicketsByEvent;
    private readonly IUseCaseRefreshClientTicketsByLocation _refreshClientTicketsByLocation;

    public TicketReadModelSyncConsumer(
        ILogger<TicketReadModelSyncConsumer> logger,
        IUseCaseProjectClientTicketsFromOrder projectClientTicketsFromOrder,
        IUseCaseProjectClientTicketFromIssuedTicket projectClientTicketFromIssuedTicket,
        IUseCaseRefreshClientTicketsByEvent refreshClientTicketsByEvent,
        IUseCaseRefreshClientTicketsByLocation refreshClientTicketsByLocation)
    {
        _logger = logger;
        _projectClientTicketsFromOrder = projectClientTicketsFromOrder;
        _projectClientTicketFromIssuedTicket = projectClientTicketFromIssuedTicket;
        _refreshClientTicketsByEvent = refreshClientTicketsByEvent;
        _refreshClientTicketsByLocation = refreshClientTicketsByLocation;
    }

    public bool Consume(TicketReadModelSyncIntegrationEvent message)
    {
        // Este consumer recebe um pedido generico de sincronizacao e delega
        // para o use case especifico que sabe reprojetar aquele recorte no Mongo.
        _logger.LogInformation(
            "Processando sincronizacao do read model de tickets. Tipo: {syncKind}. Referencia: {referenceId}.",
            message.SyncKind,
            message.ReferenceId);

        // Cada tipo de mensagem aponta para uma estrategia diferente de reprojecao.
        var result = message.SyncKind switch
        {
            TicketReadModelSyncKind.OrderTickets => _projectClientTicketsFromOrder.Execute(message.ReferenceId),
            TicketReadModelSyncKind.IssuedTicket => _projectClientTicketFromIssuedTicket.Execute(message.ReferenceId),
            TicketReadModelSyncKind.Event => _refreshClientTicketsByEvent.Execute(message.ReferenceId),
            TicketReadModelSyncKind.Location => _refreshClientTicketsByLocation.Execute(message.ReferenceId),
            _ => Generic.Domain.Entities.OperationResult.UnprocessableEntity(
                new Generic.Domain.Entities.MensagemErro("SyncKind", "Tipo de sincronizacao nao suportado."))
        };

        if (!result.Success)
        {
            // Retornar false permite que a infraestrutura trate a mensagem como falha no processamento.
            _logger.LogWarning(
                "Nao foi possivel sincronizar o read model de tickets. Tipo: {syncKind}. Referencia: {referenceId}. Erro: {error}",
                message.SyncKind,
                message.ReferenceId,
                result.Errors.FirstOrDefault()?.Mensagem ?? "erro nao informado");
            return false;
        }

        _logger.LogInformation(
            "Read model de tickets sincronizado com sucesso. Tipo: {syncKind}. Referencia: {referenceId}.",
            message.SyncKind,
            message.ReferenceId);
        return true;
    }
}
