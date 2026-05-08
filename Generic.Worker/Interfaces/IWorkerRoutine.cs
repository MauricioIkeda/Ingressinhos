namespace Generic.Worker.Interfaces;

public interface IWorkerRoutine // Interface que todas as rotinas devem implementar, para garantir a execução do método ExecuteAsync
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
