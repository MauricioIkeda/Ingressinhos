using Generic.Application.Dtos;
using Generic.Domain.Entities;

namespace Generic.Application.Utils.Interface;

public interface IRequestPayment
{
    Task<OperationResult<PaymentCheckoutApiDto>> CreatePayment(long orderId, decimal amount, string method);
    Task<OperationResult<PaymentTransactionApiDto>> GetPaymentById(long paymentTransactionId);
    Task<OperationResult<List<PaymentTransactionApiDto>>> GetPaymentsByOrder(long orderId);
    Task<OperationResult<PaymentTransactionApiDto>> CheckPaymentStatus(long orderId);
}
