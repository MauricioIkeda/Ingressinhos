using Generic.Worker.Interfaces;

namespace Generic.Worker.Configuration;

public class WorkerRoutineRegistration
{
    public required Type RoutineType { get; init; }
    public required WorkerRoutineOptions Options { get; init; }

    public string Name => string.IsNullOrWhiteSpace(Options.Name) ? RoutineType.Name : Options.Name;

    public IWorkerRoutine CreateRoutine(IServiceProvider serviceProvider)
    {
        return (IWorkerRoutine)serviceProvider.GetRequiredService(RoutineType); // Resolver a rotina via DI
    }
}
