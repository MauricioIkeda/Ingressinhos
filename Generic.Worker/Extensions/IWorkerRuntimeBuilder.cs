using Generic.Worker.Configuration;

namespace Generic.Worker.Extensions;

public interface IWorkerRuntimeBuilder // Builder exposto pelas extensões de DI para encadear o registro de novas rotinas.
{
    IServiceCollection Services { get; }
    WorkerRoutineRegistry Registry { get; }
}
