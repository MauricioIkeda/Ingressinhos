using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Models;
using Ingressinhos.Domain.Sales.Entities;

namespace Ingressinhos.Application.Sales.TicketReadModel.UseCases;

public class RefreshClientTicketsByEvent : IUseCaseRefreshClientTicketsByEvent
{
    private readonly IRepositorySession _repositorySession;
    private readonly IClientTicketReadModelWriter _writer;
    private readonly ClientTicketReadModelBuilder _builder;

    public RefreshClientTicketsByEvent(IRepositorySession repositorySession, IClientTicketReadModelWriter writer, ClientTicketReadModelBuilder builder)
    {
        _repositorySession = repositorySession;
        _writer = writer;
        _builder = builder;
    }

    public OperationResult Execute(long eventId)
    {
        if (eventId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Event", "Deve ser informado o identificador do evento."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var issuedTicketIds = repositoryQuery.Query<IssuedTicket>(ticket => ticket.EventId == eventId)
                .OrderBy(ticket => ticket.Id)
                .Select(ticket => ticket.Id)
                .ToList();

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
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
