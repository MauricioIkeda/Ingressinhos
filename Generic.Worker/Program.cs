using Generic.Worker.Configuration;
using Generic.Worker.Example;
using Generic.Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWorkerRuntime().AddScheduledRoutine<Test>(options =>
    {
        options.Name = "Vruum";
        options.Interval = TimeSpan.FromSeconds(30);
        options.RunOnStartup = true;
    });

var host = builder.Build();
host.Run();
