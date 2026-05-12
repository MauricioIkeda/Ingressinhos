using Generic.Infrastructure.Interfaces;
using Generic.Worker.Interfaces;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Worker.Options;

namespace Payment.Worker.Routines;

public class CancelExpiredPaymentsRoutine : IWorkerRoutine
{
    private readonly ILogger<CancelExpiredPaymentsRoutine> _logger;
    private readonly IRepositorySession _repositorySession;
    private readonly PaymentExpirationOptions _options;

    public CancelExpiredPaymentsRoutine(
        ILogger<CancelExpiredPaymentsRoutine> logger,
        IRepositorySession repositorySession,
        PaymentExpirationOptions options)
    {
        _logger = logger;
        _repositorySession = repositorySession;
        _options = options;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var cancelAfterMinutes = Math.Max(1, _options.CancelAfterMinutes);
        var expirationThreshold = DateTime.UtcNow.AddMinutes(-cancelAfterMinutes);

        var repositoryQuery = _repositorySession.GetRepositoryQuery();
        var expiredTransactions = repositoryQuery.Query<PaymentTransaction>(payment =>
                payment.Status == PaymentStatus.Requested &&
                payment.RequestedAt <= expirationThreshold)
            .OrderBy(payment => payment.RequestedAt)
            .ToList();

        if (expiredTransactions.Count == 0)
        {
            _logger.LogInformation(
                "Nenhuma transacao expirada para cancelar. Limite atual: {cancelAfterMinutes} minuto(s).",
                cancelAfterMinutes);
            return Task.CompletedTask;
        }

        _logger.LogInformation(
            "Encontradas {count} transacoes expiradas para cancelar. Limite atual: {cancelAfterMinutes} minuto(s).",
            expiredTransactions.Count,
            cancelAfterMinutes);

        var repository = _repositorySession.GetRepository();
        var cancelledCount = 0;

        foreach (var transaction in expiredTransactions)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            transaction.Cancel();
            if (!transaction.IsValid)
            {
                _logger.LogWarning(
                    "Nao foi possivel cancelar a transacao {paymentTransactionId} do pedido {orderId}: {error}",
                    transaction.Id,
                    transaction.OrderId,
                    transaction.Errors.FirstOrDefault()?.Mensagem ?? "erro nao informado");
                continue;
            }

            repository.Upsert(transaction);
            cancelledCount++;
        }

        if (cancelledCount == 0)
        {
            _logger.LogInformation("Nenhuma transacao expirada foi cancelada nesta execucao.");
            return Task.CompletedTask;
        }

        repository.Flush().GetAwaiter().GetResult();

        _logger.LogInformation(
            "{count} transacao(oes) expirada(s) foram canceladas com sucesso.",
            cancelledCount);

        return Task.CompletedTask;
    }
}
