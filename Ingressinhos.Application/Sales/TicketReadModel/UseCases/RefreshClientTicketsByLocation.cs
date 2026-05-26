using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Models;
using Ingressinhos.Domain.Sales.Entities;
using EventDomain = Ingressinhos.Domain.Catalog.Entities.Event;

namespace Ingressinhos.Application.Sales.TicketReadModel.UseCases;

public class RefreshClientTicketsByLocation : IUseCaseRefreshClientTicketsByLocation
{
    private readonly IRepositorySession _repositorySession;
    private readonly IClientTicketReadModelWriter _writer;
    private readonly ClientTicketReadModelBuilder _builder;

    public RefreshClientTicketsByLocation(IRepositorySession repositorySession, IClientTicketReadModelWriter writer, ClientTicketReadModelBuilder builder)
    {
        _repositorySession = repositorySession;
        _writer = writer;
        _builder = builder;
    }

    public OperationResult Execute(long locationId)
    {
        if (locationId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Location", "Deve ser informado o identificador da localizacao."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var eventIds = repositoryQuery.Query<EventDomain>(eventEntity => eventEntity.LocationId == locationId)
                .Select(eventEntity => eventEntity.Id)
                .ToArray();

            if (eventIds.Length == 0)
            {
                return OperationResult.Ok();
            }

            var issuedTicketIds = repositoryQuery.Query<IssuedTicket>(ticket => eventIds.Contains(ticket.EventId))
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
