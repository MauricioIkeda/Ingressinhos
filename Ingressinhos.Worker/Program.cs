using Generic.Worker.Extensions;
using Ingressinhos.Worker.Extensions;
using Ingressinhos.Worker.Routines;

var builder = Host.CreateApplicationBuilder(args);

// Este host existe para reagir a eventos de negocio do lado do Ingressinhos.
builder.Services.AddIngressinhosWorkerServices(builder.Configuration, builder.Environment.ContentRootPath);
builder.Services
    .AddWorkerRuntime()
    // O worker fica consultando a fila fake para processar pagamentos aprovados.
    .AddScheduledRoutine<ConsumePaymentApprovedMessagesRoutine>(options =>
    {
        options.Name = "consume-payment-approved";
        options.Interval = TimeSpan.FromSeconds(5);
        options.RunOnStartup = true;
    });

var host = builder.Build();
host.Run();
