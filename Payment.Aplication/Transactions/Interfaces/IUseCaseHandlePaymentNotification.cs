using Generic.Domain.Entities;
using Payment.Aplication.Transactions.Dtos;

namespace Payment.Aplication.Transactions.Interfaces;

public interface IUseCaseHandlePaymentNotification
{
    OperationResult Execute(PaymentNotificationDto command);
}
