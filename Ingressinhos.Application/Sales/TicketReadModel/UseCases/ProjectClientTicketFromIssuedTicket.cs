using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

namespace Ingressinhos.Application.Sales.TicketReadModel.UseCases;

public class ProjectClientTicketFromIssuedTicket : IUseCaseProjectClientTicketFromIssuedTicket
{
    private readonly IRepositorySession _repositorySession;
    private readonly IClientTicketReadModelWriter _writer;
    private readonly ClientTicketReadModelBuilder _builder;

    public ProjectClientTicketFromIssuedTicket(IRepositorySession repositorySession, IClientTicketReadModelWriter writer, ClientTicketReadModelBuilder builder)
    {
        _repositorySession = repositorySession;
        _writer = writer;
        _builder = builder;
    }

    public OperationResult Execute(long issuedTicketId)
    {
        if (issuedTicketId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("IssuedTicket", "Deve ser informado o identificador do bilhete."));
        }

        try
        {
            var buildResult = _builder.Build(issuedTicketId, _repositorySession.GetRepositoryQuery());
            if (!buildResult.Success)
            {
                return OperationResult.Fail(buildResult.Errors);
            }

            _writer.Upsert(buildResult.Data);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
