using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Dtos;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Models;
using Ingressinhos.Domain.Sales.Entities;

namespace Ingressinhos.Application.Sales.TicketReadModel.UseCases;

public class BackfillClientTicketsReadModel : IUseCaseBackfillClientTicketsReadModel
{
    private readonly IRepositorySession _repositorySession;
    private readonly IClientTicketReadModelWriter _writer;
    private readonly ClientTicketReadModelBuilder _builder;

    public BackfillClientTicketsReadModel(IRepositorySession repositorySession, IClientTicketReadModelWriter writer, ClientTicketReadModelBuilder builder)
    {
        _repositorySession = repositorySession;
        _writer = writer;
        _builder = builder;
    }

    public OperationResult<BackfillClientTicketsReadModelResult> Execute(int batchSize)
    {
        if (batchSize <= 0)
        {
            batchSize = 100;
        }

        try
        {
            IRepositoryQuery repositoryQuery = _repositorySession.GetRepositoryQuery();
            int projectedCount = 0;
            long lastId = 0;

            while (true)
            {
                var issuedTicketIds = repositoryQuery.Query<IssuedTicket>(ticket => ticket.Id > lastId)
                    .OrderBy(ticket => ticket.Id)
                    .Select(ticket => ticket.Id)
                    .Take(batchSize)
                    .ToList();

                if (issuedTicketIds.Count == 0)
                {
                    break;
                }

                var entries = new List<ClientTicketReadModelEntry>();
                foreach (var issuedTicketId in issuedTicketIds)
                {
                    var buildResult = _builder.Build(issuedTicketId, repositoryQuery);
                    if (!buildResult.Success)
                    {
                        return OperationResult<BackfillClientTicketsReadModelResult>.FromResult(OperationResult.Fail(buildResult.Errors));
                    }

                    entries.Add(buildResult.Data);
                    lastId = issuedTicketId;
                }

                _writer.UpsertMany(entries);
                projectedCount += entries.Count;
            }

            return OperationResult<BackfillClientTicketsReadModelResult>.Ok(new BackfillClientTicketsReadModelResult
            {
                ProjectedCount = projectedCount
            });
        }
        catch (Exception ex)
        {
            return OperationResult<BackfillClientTicketsReadModelResult>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
