using Generic.Worker.Extensions;
using Ingressinhos.Worker.Extensions;
using Ingressinhos.Worker.Routines;

var builder = Host.CreateApplicationBuilder(args);

// Este host existe para reagir a eventos de negocio do lado do Ingressinhos.
builder.Services.AddIngressinhosWorkerServices(builder.Configuration, builder.Environment.ContentRootPath);
builder.Services
    .AddWorkerRuntime()
    // O worker reage aos eventos de pagamento publicados pelo contexto Payment.
    .AddScheduledRoutine<ConsumePaymentMessagesRoutine>(options =>
    {
        options.Name = "consume-payment-messages";
        options.Interval = TimeSpan.FromSeconds(5);
        options.RunOnStartup = true;
    })
    .AddScheduledRoutine<ConsumeTicketReadModelMessagesRoutine>(options =>
    {
        options.Name = "consume-ticket-read-model-messages";
        options.Interval = TimeSpan.FromSeconds(5);
        options.RunOnStartup = true;
    })
    .AddScheduledRoutine<BackfillClientTicketReadModelRoutine>(options =>
    {
        options.Name = "backfill-client-ticket-read-model";
        options.Interval = TimeSpan.FromHours(1);
        options.RunOnStartup = true;
    });

var host = builder.Build();
host.Run();
