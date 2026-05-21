using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Helpers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Enums;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

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
            var client = CurrentUserEntityResolver.ResolveClient(_currentUserContext, repositoryQuery, command.ClientId);
            if (client is null)
            {
                return _currentUserContext.Role == "Admin"
                    ? OperationResult.NotFound(new MensagemErro("Cliente", "Cliente informado nao foi encontrado."))
                    : OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
            }

            var utcNow = DateTime.UtcNow;
            var repository = _repositorySession.GetRepository();
            using var transaction = _repositorySession.BeginTransaction();

            // Cada cliente pode manter apenas um pedido em carrinho.
            // Se ele ja existir, este fluxo reaproveita o pedido e substitui seus itens.
            var orderEntity = repositoryQuery
                .Query<OrderDomain>(order => order.ClientId == client.Id && order.Status == OrderStatus.Cart)
                .OrderByDescending(order => order.Id)
                .FirstOrDefault();
            if (orderEntity is null)
            {
                orderEntity = new OrderDomain(client.Id)
                {
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                };

                if (!orderEntity.IsValid)
                {
                    return orderEntity.ToUnprocessableEntityResult();
                }

                repository.Include(orderEntity);
                repository.Flush().GetAwaiter().GetResult();
            }

            // O helper sincroniza o pedido inteiro com a lista recebida:
            // remove os itens antigos e recria os OrderItems a partir do request atual.
            var saveResult = OrderItemSyncHelper.Sync(orderEntity, command.Items, repository, repositoryQuery, utcNow);
            if (!saveResult.Success)
            {
                _repositorySession.RollbackTransaction();
                return saveResult;
            }

            repository.Flush().GetAwaiter().GetResult();
            _repositorySession.CommitTransaction();

            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _repositorySession.RollbackTransaction();
            return OperationResult.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
