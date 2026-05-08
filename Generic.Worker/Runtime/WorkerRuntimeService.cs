using System.Diagnostics;
using Generic.Worker.Configuration;

namespace Generic.Worker.Runtime;

public class WorkerRuntimeService : BackgroundService
{
    private static readonly TimeSpan SchedulerTick = TimeSpan.FromSeconds(5); // verificar quais podem ser executadas

    private readonly IServiceProvider _serviceProvider;
    private readonly WorkerRoutineRegistry _registry;
    private readonly ILogger<WorkerRuntimeService> _logger;

    private readonly Dictionary<string, DateTimeOffset> _nextExecutions = []; // horario da próxima execução de cada rotina

    private readonly HashSet<string> _runningRoutines = []; // Evitar que a mesma rotina seja executada em paralelo, caso demore mais do que o intervalo configurado.

    public WorkerRuntimeService( IServiceProvider serviceProvider, WorkerRoutineRegistry registry, ILogger<WorkerRuntimeService> logger)
    {
        _serviceProvider = serviceProvider;
        _registry = registry;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitializeSchedule();

        if (_registry.Registrations.Count == 0)
        {
            _logger.LogWarning("Nenhuma rotina agendada foi registrada no worker.");
        }
        else
        {
            _logger.LogInformation("Worker iniciado com {count} rotina(s) agendada(s).", _registry.Registrations.Count);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var registration in _registry.Registrations)
            {
                var routineName = registration.Name;
                
                if (_runningRoutines.Contains(routineName))
                {
                    continue;
                }

                if (!_nextExecutions.TryGetValue(routineName, out var nextExecution)) // Se por algum motivo a rotina não tiver um horário agendado
                {
                    nextExecution = DateTimeOffset.UtcNow;
                }

                if (DateTimeOffset.UtcNow < nextExecution) // Ainda não é hora de executar essa rotina
                {
                    continue;
                }

                await ExecuteRoutineAsync(registration, stoppingToken); // A execução
            }

            await Task.Delay(SchedulerTick, stoppingToken);
        }
    }

    private void InitializeSchedule()
    {
        var utcNow = DateTimeOffset.UtcNow;

        foreach (var registration in _registry.Registrations)
        {
            _nextExecutions[registration.Name] = registration.Options.RunOnStartup ? utcNow : utcNow.Add(registration.Options.Interval);
        }
    }

    private async Task ExecuteRoutineAsync(WorkerRoutineRegistration registration, CancellationToken cancellationToken) // => Evitar que uma rotina atrase as outras
    {
        var routineName = registration.Name;
        _runningRoutines.Add(routineName);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var routine = registration.CreateRoutine(scope.ServiceProvider);
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Iniciando rotina {routineName}.", routineName);

            await routine.ExecuteAsync(cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("Rotina {routineName} finalizada em {elapsedMs} ms.", routineName, stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cancelando rotina {routineName}.", routineName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar a rotina {routineName}.", routineName);
        }
        finally
        {
            // A próxima execução sempre é calculada a partir do fim da rodada atual.
            _nextExecutions[routineName] = DateTimeOffset.UtcNow.Add(registration.Options.Interval);
            _runningRoutines.Remove(routineName);
        }
    }
}
