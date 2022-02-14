using iss_location_ingestor.Services;

var builder = WebApplication.CreateBuilder(args);

if(builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton<TleCache>();
builder.Services.AddSingleton<EventHubSender>();
builder.Services.AddHostedService<IssPositionPropagator>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
