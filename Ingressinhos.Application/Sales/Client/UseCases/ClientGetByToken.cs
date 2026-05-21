using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Sales.Dtos;
using ClientDomain = Ingressinhos.Domain.Sales.Entities.Client;

namespace Ingressinhos.Application.Sales.UseCases;

public class ClientGetByToken
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IRepositoryQuery _query;

    public ClientGetByToken(ICurrentUserContext currentUserContext, IRepositoryQuery query)
    {
        _currentUserContext = currentUserContext;
        _query = query;
    }

    public OperationResult<ClientGet> Execute()
    {
        if (!_currentUserContext.IsAuthenticated)
        {
            return OperationResult<ClientGet>.Unauthorized(new MensagemErro("Usuario", "Usuario nao autenticado."));
        }

        var client = CurrentUserEntityResolver.ResolveClient(_currentUserContext, _query);
        if (client is null)
        {
            return OperationResult<ClientGet>.NotFound(new MensagemErro("Client", "Cliente nao encontrado"));
        }

        var clientDto = new ClientGet(client.Id, client.Name, client.Email.Endereco, client.Cpf.Numero);
        return OperationResult<ClientGet>.Ok(clientDto);
    }
}
