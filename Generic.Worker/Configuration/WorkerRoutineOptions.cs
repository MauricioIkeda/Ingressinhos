namespace Generic.Worker.Configuration;

public class WorkerRoutineOptions
{
    public string Name { get; set; } = string.Empty;
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);
    public bool RunOnStartup { get; set; } = true;
}
