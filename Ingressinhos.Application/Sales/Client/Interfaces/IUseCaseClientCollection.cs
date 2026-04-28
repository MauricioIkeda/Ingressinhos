using Generic.Application.Crud.Interface;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.Interfaces;

public interface IUseCaseClientCollection : IUseCaseCrudCollection<ClientDomain, ClientDto>
{
    
}