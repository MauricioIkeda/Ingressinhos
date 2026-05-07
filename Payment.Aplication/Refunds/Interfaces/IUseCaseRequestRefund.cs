using Generic.Domain.Entities;
using Payment.Aplication.Refunds.Dtos;

namespace Payment.Aplication.Refunds.Interfaces;

public interface IUseCaseRequestRefund
{
    OperationResult<RefundDto> Execute(RequestRefundDto command);
}
