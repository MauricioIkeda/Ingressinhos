using Generic.Worker.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;

namespace Ingressinhos.Worker.Routines;

public class BackfillClientTicketReadModelRoutine : IWorkerRoutine
{
    private static bool _hasRun;

    private readonly ILogger<BackfillClientTicketReadModelRoutine> _logger;
    private readonly IConfiguration _configuration;
    private readonly IClientTicketReadModelHealthCheck _healthCheck;
    private readonly IUseCaseBackfillClientTicketsReadModel _backfillClientTicketsReadModel;

    public BackfillClientTicketReadModelRoutine(
        ILogger<BackfillClientTicketReadModelRoutine> logger,
        IConfiguration configuration,
        IClientTicketReadModelHealthCheck healthCheck,
        IUseCaseBackfillClientTicketsReadModel backfillClientTicketsReadModel)
    {
        _logger = logger;
        _configuration = configuration;
        _healthCheck = healthCheck;
        _backfillClientTicketsReadModel = backfillClientTicketsReadModel;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (_hasRun)
        {
            return Task.CompletedTask;
        }

        _hasRun = true;

        var enabled = bool.TryParse(_configuration["TicketReadModelBackfill:EnabledOnStartup"], out var enabledValue) && enabledValue;
        if (!enabled)
        {
            _logger.LogInformation("Backfill do read model de tickets desabilitado.");
            return Task.CompletedTask;
        }

        if (!_healthCheck.IsAvailable())
        {
            _logger.LogWarning("MongoDB indisponivel. Backfill do read model de tickets nao sera executado agora.");
            return Task.CompletedTask;
        }

        var batchSize = int.TryParse(_configuration["TicketReadModelBackfill:BatchSize"], out var configuredBatchSize)
            ? configuredBatchSize
            : 100;

        var result = _backfillClientTicketsReadModel.Execute(batchSize);
        if (!result.Success)
        {
            _logger.LogWarning(
                "Backfill do read model de tickets falhou: {error}",
                result.Errors.FirstOrDefault()?.Mensagem ?? "erro nao informado");
            return Task.CompletedTask;
        }

        _logger.LogInformation(
            "Backfill do read model de tickets concluido. {count} bilhete(s) projetado(s).",
            result.Data.ProjectedCount);

        return Task.CompletedTask;
    }
}
