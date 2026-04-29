using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerUpdate : IUseCaseCommand<SellerDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;

    public SellerUpdate(IRepositorySession repositorySession, IRequestAuth requestAuth)
    {
        _repositorySession = repositorySession;
        _requestAuth = requestAuth;
    }

    public bool Execute(SellerDto seller)
    {
        Messages.Clear();

        if (seller is null)
        {
            Messages.Add("Deve ser informado o vendedor", error: true);
            return false;
        }

        if (seller.SellerId <= 0)
        {
            Messages.Add("Deve ser informado o identificador do vendedor", error: true);
            return false;
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var sellerEntity = repositoryQuery.Return<Seller>(seller.SellerId);

            if (sellerEntity is null)
            {
                Messages.Add("Vendedor nao encontrado", error: true);
                return false;
            }

            if (seller.Name != sellerEntity.Name)
                sellerEntity.ChangeName(seller.Name);

            if (seller.Email != sellerEntity.Email.Endereco)
            {
                sellerEntity.ChangeEmail(seller.Email);
                if (!_requestAuth.ChangeEmail(sellerEntity.UserId, seller.Email).GetAwaiter().GetResult())
                {
                    Messages.Add("Falha ao atualizar o email do usuario", error: true);
                    return false;
                }
            }

            if (seller.Cnpj != sellerEntity.Cnpj.Numero)
                sellerEntity.ChangeTradingName(seller.TradingName);

            sellerEntity.UpdatedAt = DateTime.UtcNow;

            var repository = _repositorySession.GetRepository();
            repository.Upsert(sellerEntity);
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
