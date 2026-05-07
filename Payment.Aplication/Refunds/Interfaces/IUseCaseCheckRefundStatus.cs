using Generic.Domain.Entities;
using Payment.Aplication.Refunds.Dtos;

namespace Payment.Aplication.Refunds.Interfaces;

public interface IUseCaseCheckRefundStatus
{
    OperationResult<RefundDto> Execute(long refundId);
}
