using Generic.Worker.Configuration;

namespace Generic.Worker.Extensions;

public class WorkerRuntimeBuilder : IWorkerRuntimeBuilder // Configuração para não precisar ficar passando o registry toda hora
{
    public IServiceCollection Services { get; }
    public WorkerRoutineRegistry Registry { get; }

    public WorkerRuntimeBuilder(IServiceCollection services, WorkerRoutineRegistry registry)
    {
        Services = services;
        Registry = registry;
    }
}
