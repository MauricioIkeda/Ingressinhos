using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Helpers;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerGetByToken
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IRepositoryQuery _query;

    public SellerGetByToken(ICurrentUserContext currentUserContext, IRepositoryQuery query)
    {
        _currentUserContext = currentUserContext;
        _query = query;
    }

    public OperationResult<SellerGet> Execute()
    {
        if (!_currentUserContext.IsAuthenticated)
        {
            return OperationResult<SellerGet>.Unauthorized(new MensagemErro("Usuario", "Usuario nao autenticado."));
        }

        var seller = CurrentUserEntityResolver.ResolveSeller(_currentUserContext, _query);
        if (seller is null)
        {
            return OperationResult<SellerGet>.NotFound(new MensagemErro("Seller", "Vendedor nao encontrado"));
        }

        var sellerDto = new SellerGet(seller.Id, seller.Name, seller.Email.Endereco, seller.Cnpj.Numero, seller.TradingName);
        return OperationResult<SellerGet>.Ok(sellerDto);
    }
}
