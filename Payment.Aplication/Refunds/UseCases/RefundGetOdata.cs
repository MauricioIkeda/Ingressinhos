using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Payment.Aplication.Refunds.Dtos;
using Payment.Domain.Entities;

namespace Payment.Aplication.Refunds.UseCases;

public class RefundGetOdata : UseCaseGetQueryItems<Refund, RefundQueryItem>
{
    public RefundGetOdata(IRepositoryQuery repositoryQuery)
        : base(repositoryQuery)
    {
    }

    protected override RefundQueryItem ToQueryItem(Refund refund)
    {
        return new RefundQueryItem
        {
            Id = refund.Id,
            PaymentTransactionId = refund.PaymentTransactionId,
            Amount = refund.Amount.Value,
            Reason = refund.Reason,
            Status = refund.Status,
            RequestedAt = refund.RequestedAt,
            CompletedAt = refund.CompletedAt,
            RejectedAt = refund.RejectedAt,
            CreatedAt = refund.CreatedAt,
            UpdatedAt = refund.UpdatedAt
        };
    }
}
