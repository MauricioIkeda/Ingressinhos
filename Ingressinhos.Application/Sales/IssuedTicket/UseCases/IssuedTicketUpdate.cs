using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Enums;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.UseCases;

public class IssuedTicketUpdate : IUseCaseCommand<IssuedTicketDto>
{
    private readonly IRepositorySession _repositorySession;

    public IssuedTicketUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(IssuedTicketDto issuedTicketDto)
    {
        if (issuedTicketDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("IssuedTicket", "Deve ser informado o ingresso emitido."));
        }

        if (issuedTicketDto.IssuedTicketId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador do ingresso emitido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var issuedTicketEntity = repositoryQuery.Return<IssuedTicketDomain>(issuedTicketDto.IssuedTicketId);

            if (issuedTicketEntity is null)
            {
                return OperationResult.NotFound(new MensagemErro("Id", "Ingresso emitido nao encontrado."));
            }

            if (issuedTicketDto.OrderItemId != issuedTicketEntity.OrderItemId ||
                issuedTicketDto.ClientId != issuedTicketEntity.ClientId ||
                issuedTicketDto.EventId != issuedTicketEntity.EventId ||
                issuedTicketDto.AccessCode != issuedTicketEntity.AccessCode)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("IssuedTicket", "Nao eh permitido alterar os dados base do ingresso emitido."));
            }

            if (issuedTicketDto.Status != issuedTicketEntity.Status)
            {
                switch (issuedTicketDto.Status)
                {
                    case IssuedTicketStatus.CheckedIn:
                        issuedTicketEntity.CheckIn();
                        if (!issuedTicketEntity.IsValid)
                        {
                            return issuedTicketEntity.ToUnprocessableEntityResult();
                        }
                        break;
                    case IssuedTicketStatus.Cancelled:
                        issuedTicketEntity.Cancel();
                        if (!issuedTicketEntity.IsValid)
                        {
                            return issuedTicketEntity.ToUnprocessableEntityResult();
                        }
                        break;
                    default:
                        return OperationResult.UnprocessableEntity(new MensagemErro("Status", "Nao eh possivel retornar o ingresso ao status emitido."));
                }
            }

            issuedTicketEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(issuedTicketEntity);
            repository.Flush().GetAwaiter().GetResult();
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
