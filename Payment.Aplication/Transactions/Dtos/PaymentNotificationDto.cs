namespace Payment.Aplication.Transactions.Dtos;

public class PaymentNotificationDto
{
    public string GatewayTransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
