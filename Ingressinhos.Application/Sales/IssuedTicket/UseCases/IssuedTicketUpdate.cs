using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Enums;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.UseCases;

public class IssuedTicketUpdate
{
    private readonly IRepositorySession _repositorySession;

    public IssuedTicketUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(IssuedTicketDto issuedTicketDto)
    {
        if (issuedTicketDto is null)
        {
            throw new Exception("Deve ser informado o ingresso emitido");
        }

        if (issuedTicketDto.IssuedTicketId <= 0)
        {
            throw new Exception("Deve ser informado o identificador do ingresso emitido");
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var issuedTicketEntity = repositoryQuery.Return<IssuedTicketDomain>(issuedTicketDto.IssuedTicketId);

        if (issuedTicketEntity is null)
        {
            throw new Exception("Ingresso emitido nao encontrado");
        }

        if (issuedTicketDto.OrderItemId != issuedTicketEntity.OrderItemId ||
            issuedTicketDto.ClientId != issuedTicketEntity.ClientId ||
            issuedTicketDto.EventId != issuedTicketEntity.EventId ||
            issuedTicketDto.AccessCode != issuedTicketEntity.AccessCode)
        {
            throw new Exception("Nao eh permitido alterar os dados base do ingresso emitido");
        }

        if (issuedTicketDto.Status != issuedTicketEntity.Status)
        {
            switch (issuedTicketDto.Status)
            {
                case IssuedTicketStatus.CheckedIn:
                    issuedTicketEntity.CheckIn();
                    break;
                case IssuedTicketStatus.Cancelled:
                    issuedTicketEntity.Cancel();
                    break;
                default:
                    throw new Exception("Nao eh possivel retornar o ingresso ao status emitido");
            }
        }

        issuedTicketEntity.UpdatedAt = DateTime.UtcNow;

        var repository = _repositorySession.GetRepository();
        repository.Upsert(issuedTicketEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}
