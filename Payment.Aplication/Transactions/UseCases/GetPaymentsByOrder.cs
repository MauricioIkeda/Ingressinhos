using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Aplication.Transactions.Utils;
using Payment.Domain.Entities;

namespace Payment.Aplication.Transactions.UseCases;

public class GetPaymentsByOrder : IUseCaseGetPaymentsByOrder
{
    private readonly IRepositorySession _repositorySession;

    public GetPaymentsByOrder(IRepositorySession repositorySession)
    {
        _repositorySession = repositorySession;
    }

    public OperationResult<IEnumerable<PaymentTransactionDto>> Execute(long orderId)
    {
        if (orderId <= 0)
        {
            return OperationResult<IEnumerable<PaymentTransactionDto>>.UnprocessableEntity(new MensagemErro("Pedido", "Deve ser informado o pedido da consulta."));
        }

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var transactions = repositoryQuery.Query<PaymentTransaction>(payment => payment.OrderId == orderId)
            .OrderByDescending(payment => payment.RequestedAt)
            .ToList()
            .Select(payment => payment.ToDto());

        return OperationResult<IEnumerable<PaymentTransactionDto>>.Ok(transactions);
    }
}
