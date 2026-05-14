using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Domain.Sales.Entities;
using OrderDomain = Ingressinhos.Domain.Sales.Entities.Order;

namespace Ingressinhos.Application.Sales.UseCases;

public class UpdateOrderItems
{
    private readonly IRepositorySession _repositorySession;
    private readonly ICurrentUserContext _currentUserContext;

    public UpdateOrderItems(IRepositorySession repositorySession, ICurrentUserContext currentUserContext)
    {
        _repositorySession = repositorySession;
        _currentUserContext = currentUserContext;
    }

    public OperationResult Execute(UpdateOrderItemsRequest command)
    {
        if (command is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Pedido", "Envie os dados do pedido."));
        }

        if (command.OrderId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Pedido", "Deve ser informado o identificador do pedido."));
        }

        if (command.Items is null)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Itens", "Envie a lista de itens do pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var order = repositoryQuery.Return<OrderDomain>(command.OrderId);
            if (order is null)
            {
                return OperationResult.NotFound(new MensagemErro("Pedido", "Pedido nao encontrado."));
            }

            if (_currentUserContext.Role != "Admin")
            {
                var client = repositoryQuery.Query<Client>(c => c.UserId == _currentUserContext.UserId && c.Active).FirstOrDefault();
                if (client is null)
                {
                    return OperationResult.Unauthorized(new MensagemErro("Perfil", "Nao foi possivel localizar o perfil da sua conta."));
                }

                if (order.ClientId != client.Id)
                {
                    return OperationResult.Forbidden(new MensagemErro("Pedido", "Voce so pode alterar itens de pedidos da sua propria conta."));
                }
            }

            order.ResetItems();
            if (!order.IsValid)
            {
                return order.ToUnprocessableEntityResult();
            }

            var utcNow = DateTime.UtcNow;
            var repository = _repositorySession.GetRepository();
            using var transaction = _repositorySession.BeginTransaction();

            foreach (var existingItem in order.Items.ToList())
            {
                repository.Delete(existingItem);
            }

            foreach (var item in command.Items)
            {
                var buildResult = CreateOrder.BuildOrderItem(order, item, repositoryQuery, utcNow);
                if (!buildResult.Success)
                {
                    _repositorySession.RollbackTransaction();
                    return CreateOrder.ToOperationResult(buildResult);
                }

                repository.Include(buildResult.Data);
            }

            order.UpdatedAt = utcNow;
            repository.Upsert(order);
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
