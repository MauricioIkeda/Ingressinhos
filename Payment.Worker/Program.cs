using Generic.Worker.Extensions;
using Payment.Worker.Extensions;
using Payment.Worker.Routines;

var builder = Host.CreateApplicationBuilder(args);

// Este host existe para executar rotinas do contexto Payment sem passar pela API HTTP.
builder.Services.AddPaymentWorkerServices(builder.Configuration, builder.Environment.ContentRootPath);
builder.Services
    .AddWorkerRuntime()
    // Cancela transacoes que ficaram Requested por tempo demais sem retorno final do gateway.
    .AddScheduledRoutine<CancelExpiredPaymentsRoutine>(options =>
    {
        options.Name = "cancel-expired-payments";
        options.Interval = TimeSpan.FromMinutes(5);
        options.RunOnStartup = true;
    });

var host = builder.Build();
host.Run();
