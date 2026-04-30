using Generic.Application.Crud.Interface;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Entities;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class OrderInclude : IUseCaseCommand<OrderDto>
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public OrderInclude(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(OrderDto orderDto)
    {
        if (orderDto is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Pedido", "Envie os dados do pedido."));
        }
        
        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var client = repositoryQuery.Query<Client>(c => c.UserId == _currentUserContext.UserId).FirstOrDefault();

            if (client is null)
            {
                return OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
            }

            var utcNow = DateTime.UtcNow;

            var orderEntity = new OrderDomain(client.Id)
            {
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
            if (!orderEntity.IsValid)
            {
                return orderEntity.ToUnprocessableEntityResult();
            }

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
