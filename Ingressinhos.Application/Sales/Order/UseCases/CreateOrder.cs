using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Entities;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;
using OrderItemDomain = Ingressinhos.Domain.Sales.Entities.OrderItem;

namespace Ingressinhos.Application.Sales.UseCases;

public class CreateOrder
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public CreateOrder(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(CreateOrderRequest command)
    {
        if (command is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Pedido", "Envie os dados do pedido."));
        }

        if (command.Items is null || command.Items.Count == 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Itens", "Informe ao menos um item para criar o pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var client = ResolveClient(command.ClientId, repositoryQuery);
            if (client is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.NotFound(new MensagemErro("Cliente", "Cliente informado nao foi encontrado."))
                    : OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
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
            using var transaction = _repositorySession.BeginTransaction();

            repository.Include(orderEntity);
            repository.Flush().GetAwaiter().GetResult();

            foreach (var item in command.Items)
            {
                var buildResult = BuildOrderItem(orderEntity, item, repositoryQuery, utcNow);
                if (!buildResult.Success)
                {
                    _repositorySession.RollbackTransaction();
                    return buildResult;
                }

                repository.Include(buildResult.Data);
            }

            repository.Upsert(orderEntity);
            repository.Flush().GetAwaiter().GetResult();
            _repositorySession.CommitTransaction();

            return OperationResult.Created();
        }
        catch (Exception ex)
        {
            _repositorySession.RollbackTransaction();
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

    private Client? ResolveClient(long clientId, IRepositoryQuery repositoryQuery)
    {
        if (_currentUserContext.Role == "Admin")
        {
            if (clientId <= 0)
            {
                return null;
            }

            return repositoryQuery.Query<Client>(c => c.Id == clientId && c.Active).FirstOrDefault();
        }

        return repositoryQuery.Query<Client>(c => c.UserId == _currentUserContext.UserId && c.Active).FirstOrDefault();
    }

    internal static OperationResult<OrderItemDomain> BuildOrderItem(
        OrderDomain order,
        OrderItemRequest item,
        IRepositoryQuery repositoryQuery,
        DateTime utcNow)
    {
        if (item is null)
        {
            return OperationResult<OrderItemDomain>.UnprocessableEntity(new MensagemErro("Item", "Envie os dados do item do pedido."));
        }

        var ticket = repositoryQuery.Return<Ticket>(item.TicketId);
        if (ticket is null)
        {
            return OperationResult<OrderItemDomain>.NotFound(new MensagemErro("Ingresso", "Ingresso informado nao foi encontrado."));
        }

        // Precisamos considerar categoria/assento quando essa escolha estiver modelada no carrinho.
        var unitPrice = ticket.BasePrice.Value;
        order.AddItem(unitPrice, item.Quantity);
        if (!order.IsValid)
        {
            return OperationResult<OrderItemDomain>.FromResult(order.ToUnprocessableEntityResult());
        }

        var orderItemEntity = new OrderItemDomain(
            order.Id,
            ticket.Id,
            ticket.Name,
            item.Quantity,
            unitPrice)
        {
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        if (!orderItemEntity.IsValid)
        {
            return OperationResult<OrderItemDomain>.FromResult(orderItemEntity.ToUnprocessableEntityResult());
        }

        return OperationResult<OrderItemDomain>.Created(orderItemEntity);
    }

    internal static OperationResult ToOperationResult<T>(OperationResult<T> result)
    {
        return result.StatusCode switch
        {
            401 => OperationResult.Unauthorized(result.Errors.First()),
            403 => OperationResult.Forbidden(result.Errors.First()),
            404 => OperationResult.NotFound(result.Errors.First()),
            422 => OperationResult.UnprocessableEntity(result.Errors),
            500 => OperationResult.FatalError(result.Errors.First()),
            _ => OperationResult.Fail(result.Errors)
        };
    }
}
