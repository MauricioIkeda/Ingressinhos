using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientGetOdata : UseCaseGetQueryItems<ClientDomain, ClientQueryItem>
{
    public ClientGetOdata(IReadRepositoryQuery readRepositoryQuery)
        : base(readRepositoryQuery)
    {
    }

    protected override ClientQueryItem ToQueryItem(ClientDomain client)
    {
        return new ClientQueryItem
        {
            Id = client.Id,
            UserId = client.UserId,
            Name = client.Name,
            Email = client.Email.Endereco,
            Active = client.Active,
            Cpf = client.Cpf.Numero,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt
        };
    }
}
