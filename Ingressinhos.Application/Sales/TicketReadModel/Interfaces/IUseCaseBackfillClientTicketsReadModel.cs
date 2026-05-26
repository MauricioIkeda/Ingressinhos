using Generic.Domain.Entities;
using Ingressinhos.Application.Sales.TicketReadModel.Dtos;

namespace Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

public interface IUseCaseBackfillClientTicketsReadModel // Reconstroi o mongo com base no Postgres
{
    OperationResult<BackfillClientTicketsReadModelResult> Execute(int batchSize);
}
