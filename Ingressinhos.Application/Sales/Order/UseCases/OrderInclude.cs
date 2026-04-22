using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Entities;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderInclude
{
    private readonly IRepositorySession _repositorySession;

    public OrderInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public bool Execute(OrderDto orderDto)
    {
        if (orderDto is null)
        {
            throw new Exception("Deve ser informado o pedido");
        }
        
        var client = _repositorySession.GetRepositoryQuery().Return<Client>(orderDto.ClientId);

        if (client is null)
        {
            throw new Exception("Deve ser informado o cliente");
        }

        var utcNow = DateTime.UtcNow;

        var orderEntity = new OrderDomain(orderDto.ClientId)
        {
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        var repository = _repositorySession.GetRepository();
        repository.Include(orderEntity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}
