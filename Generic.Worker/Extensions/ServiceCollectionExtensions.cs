using Generic.Worker.Interfaces;
using Generic.Worker.Configuration;
using Generic.Worker.Runtime;

namespace Generic.Worker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IWorkerRuntimeBuilder AddWorkerRuntime(this IServiceCollection services)
    {
        var registry = new WorkerRoutineRegistry();

        services.AddSingleton(registry);
        services.AddHostedService<WorkerRuntimeService>();

        return new WorkerRuntimeBuilder(services, registry);
    }

    public static IWorkerRuntimeBuilder AddScheduledRoutine<TRoutine>( this IWorkerRuntimeBuilder builder, Action<WorkerRoutineOptions>? configure = null)
        where TRoutine : class, IWorkerRoutine  // Configurar na aplicação
    {
        var options = new WorkerRoutineOptions
        {
            Name = typeof(TRoutine).Name  // configurar na aplicação
        };

        configure?.Invoke(options);

        if (options.Interval <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("O intervalo da rotina deve ser maior que zero."); //  Evitar minha burrice
        }

        builder.Services.AddScoped<TRoutine>();
        builder.Registry.Add(new WorkerRoutineRegistration // Configurar na aplicação
        {
            RoutineType = typeof(TRoutine),
            Options = options
        });

        return builder;
    }
}
