using Generic.Worker.Interfaces;

namespace Generic.Worker.Example;

public class Test : IWorkerRoutine
{
    private readonly ILogger<Test> _logger;

    public Test(ILogger<Test> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Exemplo meio danese, para testar
        _logger.LogInformation("Teste executado em: {time}", DateTimeOffset.Now);
        return Task.CompletedTask;
    }
}
