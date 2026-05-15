using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerGetOdata : UseCaseGetQueryItems<Seller, SellerQueryItem>
{
    public SellerGetOdata(IRepositoryQuery repositoryQuery)
        : base(repositoryQuery)
    {
    }

    protected override SellerQueryItem ToQueryItem(Seller seller)
    {
        return new SellerQueryItem
        {
            Id = seller.Id,
            UserId = seller.UserId,
            Name = seller.Name,
            Email = seller.Email.Endereco,
            Active = seller.Active,
            Cnpj = seller.Cnpj.Numero,
            TradingName = seller.TradingName,
            CreatedAt = seller.CreatedAt,
            UpdatedAt = seller.UpdatedAt
        };
    }
}
