using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;

namespace Ingressinhos.Application.Catalog.UseCases;

public class SellerInclude : IUseCaseCommand<SellerDto>
{
    public ListMessages Messages { get; } = new();

    private readonly IRepositorySession _repositorySession;
    private readonly IRequestAuth _requestAuth;

    public SellerInclude(IRepositorySession repositorySession, IRequestAuth requestAuth)
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

        try
        {
            string userId = _requestAuth.CreateUser(seller.Name, seller.Email, seller.Password, 1)
                .GetAwaiter().GetResult();

            var utcNow = DateTime.UtcNow;

            var sellerEntity = new Seller(seller.Name, seller.Email, seller.Cnpj, seller.TradingName, userId)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };

            var repository = _repositorySession.GetRepository();
            repository.Include(sellerEntity);
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
