using iss_location_ingestor.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) => builder.AddJsonFile("appsettings.Development.json", true).AddJsonFile("appsettings.local.json", true).AddEnvironmentVariables())               
    .ConfigureServices(services =>
    {
        services.AddSingleton<TleCache>();
        services.AddSingleton<EventHubSender>();
        services.AddHostedService<IssPositionPropagator>();
    })
    .Build();

await host.RunAsync();
