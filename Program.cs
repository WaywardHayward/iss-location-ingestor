using System.Diagnostics.Metrics;

using iss_location_ingestor.Services;
using iss_location_ingestor.Utils;

var builder = WebApplication.CreateBuilder(args);

if(builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSingleton(m => new Meter("iss.ingestor", "v1.0"));
builder.Services.AddSingleton<TleCache>();
builder.Services.AddUpstreams();
builder.Services.AddHostedService<IssPositionPropagator>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
