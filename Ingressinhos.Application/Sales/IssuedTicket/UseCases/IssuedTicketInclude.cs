using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Entities;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.UseCases;

public class IssuedTicketInclude : IUseCaseCommand<IssuedTicketDto>
{
    private readonly IRepositorySession _repositorySession;

    public IssuedTicketInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(IssuedTicketDto issuedTicketDto)
    {
        if (issuedTicketDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("IssuedTicket", "Deve ser informado o ingresso emitido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();

            if (repositoryQuery.Return<OrderItem>(issuedTicketDto.OrderItemId) is null)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("OrderItemId", "Deve ser informado um pedido valido."));
            }
            
            if (repositoryQuery.Return<Client>(issuedTicketDto.ClientId) is null)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("ClientId", "Deve ser informado um cliente valido."));
            }
            
            if (repositoryQuery.Return<Event>(issuedTicketDto.EventId) is null)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("EventId", "Deve ser informado um evento valido."));
            }

            var utcNow = DateTime.UtcNow;
            
            var issuedTicketEntity = new IssuedTicketDomain(
                issuedTicketDto.OrderItemId,
                issuedTicketDto.ClientId,
                issuedTicketDto.EventId,
                issuedTicketDto.AccessCode)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
            if (!issuedTicketEntity.IsValid)
            {
                return issuedTicketEntity.ToUnprocessableEntityResult();
            }

            var repository = _repositorySession.GetRepository();
            repository.Include(issuedTicketEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
