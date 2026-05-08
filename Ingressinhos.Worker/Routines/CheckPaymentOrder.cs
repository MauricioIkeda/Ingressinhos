using Generic.Application.Dtos;
using Generic.Application.Utils.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Generic.Worker.Example;
using Generic.Worker.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Ingressinhos.Domain.Sales.Enums;

namespace Ingressinhos.Worker.Routines
{
    public class CheckPaymentOrder : IWorkerRoutine
    {
        private readonly ILogger<Test> _logger;
        private readonly IRepositoryQuery _repositoryQuery;
        private readonly IRequestPayment _requestPayment;

        public CheckPaymentOrder(ILogger<Test> logger, IRepositoryQuery repositoryQuery, IRequestPayment requestPay)
        {
            _logger = logger;
            _repositoryQuery = repositoryQuery;
            _requestPayment = requestPay;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            List<Order> orders = _repositoryQuery.Query<Order>(x => x.Status == OrderStatus.PendingPayment).ToList();
            if (orders.Count == 0)
            {
                _logger.LogInformation("Nenhum pedido com pagamento pendente encontrado em: {time}", DateTimeOffset.Now);
                return Task.CompletedTask;
            }
            _logger.LogInformation("Encontrados {count} pedidos com pagamento pendente em: {time}", orders.Count, DateTimeOffset.Now);
            foreach (Order order in orders)
            {
                Task<OperationResult<PaymentTransactionApiDto>> check = _requestPayment.CheckPaymentStatus(order.Id);
                if (check.IsCompleted)
                {
                    OperationResult<PaymentTransactionApiDto> result = check.Result;
                    if (result.Success)
                    {
                        PaymentTransactionApiDto paymentInfo = result.Data;
                        if (paymentInfo.Status == (int)OrderStatus.Paid)
                        {

                            _logger.LogInformation("Pedido {orderId} pago. Atualizando status para Pago.", order.Id);
                            // Aqui você pode adicionar a lógica para atualizar o status do pedido para Pago, por exemplo:
                            // order.Status = OrderStatus.Paid;
                            // _repositoryCommand.Update(order);
                        }
                        else
                        {
                            _logger.LogInformation("Pedido {orderId} ainda não foi pago. Status atual: {status}.", order.Id, paymentInfo.Status);
                        }
                    }
                    else
                    {
                        _logger.LogError("Erro ao verificar pagamento do pedido {orderId}: {errorMessage}", order.Id, result.Errors.FirstOrDefault()?.Mensagem); // O correto era ter um log no banco de dados também, mas já estamos indo muito over
                    }
                }

            }
            return Task.CompletedTask;
        }
    }
}
