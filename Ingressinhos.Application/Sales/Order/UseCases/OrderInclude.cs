using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Entities;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderInclude : IUseCaseCommand<OrderDto>
{
    private readonly IRepositorySession _repositorySession;

    public OrderInclude(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult Execute(OrderDto orderDto)
    {
        if (orderDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Order", "Deve ser informado o pedido."));
        }
        
        try
        {
            var client = _repositorySession.GetRepositoryQuery().Return<Client>(orderDto.ClientId);

            if (client is null)
            {
                return OperationResult.UnprocessableEntity(new MensagemErro("ClientId", "Deve ser informado o cliente."));
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
            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
