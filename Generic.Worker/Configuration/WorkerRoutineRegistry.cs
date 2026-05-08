namespace Generic.Worker.Configuration;

public class WorkerRoutineRegistry // Armazena em memória todas as rotinas registradas no startup.
{
    private readonly List<WorkerRoutineRegistration> _registrations = [];

    public IReadOnlyCollection<WorkerRoutineRegistration> Registrations => _registrations;

    public void Add(WorkerRoutineRegistration registration) 
    {
        _registrations.Add(registration);
    }
}
