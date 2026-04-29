using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Enums;
using IssuedTicketDomain = Ingressinhos.Domain.Sales.Entities.IssuedTicket;

namespace Ingressinhos.Application.Sales.UseCases;

public class IssuedTicketUpdate : IUseCaseCommand<IssuedTicketDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;

    public IssuedTicketUpdate(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(IssuedTicketDto issuedTicketDto)
    {
        Messages.Clear();

        if (issuedTicketDto is null)
        {
            Messages.Add("Deve ser informado o ingresso emitido", error: true);
            return false;
        }

        if (issuedTicketDto.IssuedTicketId <= 0)
        {
            Messages.Add("Deve ser informado o identificador do ingresso emitido", error: true);
            return false;
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var issuedTicketEntity = repositoryQuery.Return<IssuedTicketDomain>(issuedTicketDto.IssuedTicketId);

            if (issuedTicketEntity is null)
            {
                Messages.Add("Ingresso emitido nao encontrado", error: true);
                return false;
            }

            if (issuedTicketDto.OrderItemId != issuedTicketEntity.OrderItemId ||
                issuedTicketDto.ClientId != issuedTicketEntity.ClientId ||
                issuedTicketDto.EventId != issuedTicketEntity.EventId ||
                issuedTicketDto.AccessCode != issuedTicketEntity.AccessCode)
            {
                Messages.Add("Nao eh permitido alterar os dados base do ingresso emitido", error: true);
                return false;
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
                        Messages.Add("Nao eh possivel retornar o ingresso ao status emitido", error: true);
                        return false;
                }
            }

            issuedTicketEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(issuedTicketEntity);
            repository.Flush().GetAwaiter().GetResult();
            return true;
        }
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }
}
