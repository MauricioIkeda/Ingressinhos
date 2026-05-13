using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseClientCollection : IUseCaseCrudCollection<ClientDomain, ClientDto>
{
    OperationResult Deactivate(long id);
    OperationResult Recover(long id);
    OperationResult<ClientGet> GetByToken();
}
