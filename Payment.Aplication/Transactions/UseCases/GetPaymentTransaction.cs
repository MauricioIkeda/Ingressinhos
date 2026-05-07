using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Aplication.Transactions.Utils;
using Payment.Domain.Entities;

namespace Payment.Aplication.Transactions.UseCases;

public class GetPaymentTransaction : IUseCaseGetPaymentTransaction
{
    private readonly IRepositorySession _repositorySession;

    public GetPaymentTransaction(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult<PaymentTransactionDto> Execute(long paymentTransactionId)
    {
        if (paymentTransactionId <= 0)
        {
            return OperationResult<PaymentTransactionDto>.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador da transacao."));
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var transaction = repositoryQuery.Return<PaymentTransaction>(paymentTransactionId);

        if (transaction is null)
        {
            return OperationResult<PaymentTransactionDto>.NotFound(new MensagemErro("Pagamento", "Transacao de pagamento nao encontrada."));
        }

        return OperationResult<PaymentTransactionDto>.Ok(transaction.ToDto());
    }
}
