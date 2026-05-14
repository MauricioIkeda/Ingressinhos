using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Aplication.Transactions.Utils;
using Payment.Domain.Entities;

namespace Payment.Aplication.Transactions.UseCases;

public class CheckPaymentStatus : IUseCaseCheckPaymentStatus
{
    private readonly IRepositorySession _repositorySession;

    public CheckPaymentStatus(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult<PaymentTransactionDto> Execute(long orderId)
    {
        if (orderId <= 0)
        {
            return OperationResult<PaymentTransactionDto>.UnprocessableEntity(new MensagemErro("Pedido", "Deve ser informado o identificador do pedido."));
        }

        try
        {
            var repositoryQuery = _repositorySession.GetRepositoryQuery();
            var transaction = repositoryQuery.Query<PaymentTransaction>(payment => payment.OrderId == orderId)
                .OrderByDescending(payment => payment.RequestedAt)
                .ThenByDescending(payment => payment.Id)
                .FirstOrDefault();

            if (transaction is null)
            {
                return OperationResult<PaymentTransactionDto>.NotFound(new MensagemErro("Pagamento", "Nenhuma transacao de pagamento foi encontrada para o pedido informado."));
            }

            return OperationResult<PaymentTransactionDto>.Ok(transaction.ToDto());
        }
        catch (Exception ex)
        {
            return OperationResult<PaymentTransactionDto>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
