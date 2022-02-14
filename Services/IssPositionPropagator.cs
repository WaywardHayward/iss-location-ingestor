
using iss_location_ingestor.Model;
using Newtonsoft.Json;
using One_Sgp4;

namespace iss_location_ingestor.Services;

public class IssPositionPropagator : BackgroundService
{
    private readonly ILogger<IssPositionPropagator> _logger;
    private readonly int _timeoutSeconds;
    private readonly int _tleRefreshInterval;
    private readonly EventHubSender _sender;
    public IConfiguration _config;
    private DateTime _lastTleRefresh;
    private readonly TleCache _tles;

    public IssPositionPropagator(ILogger<IssPositionPropagator> logger, IConfiguration config, TleCache tles, EventHubSender sender)
    {
        _tles = tles;
        _config = config;
        _logger = logger;
        _timeoutSeconds = int.TryParse(_config["TIMEOUT_SECONDS"], out var to) ? to : 5;
        _tleRefreshInterval = int.TryParse(_config["TLE_REFRESH_INTERVAL"], out var tleRefreshInterval) ? tleRefreshInterval : 60;
        _sender = sender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var tle = await GetTLE();

            EmitPosition(tle);

            await Task.Delay(TimeSpan.FromSeconds(_timeoutSeconds), stoppingToken);
        }
    }

    private void EmitPosition(Tle tle)
    {
        EpochTime startTime = new EpochTime(DateTime.UtcNow);
        var data = SatFunctions.getSatPositionAtTime(tle, startTime,  Sgp4.wgsConstant.WGS_84);
        _logger.Log(LogLevel.Information, "Emitted position {data.x} {data.y} {data.z}", data.getX(), data.getY(), data.getZ());
        var ll = SatFunctions.calcSatSubPoint(startTime,  data, Sgp4.wgsConstant.WGS_84); 
        _logger.Log(LogLevel.Information, "Emitted position {ll.lat} {ll.lon}", ll.getLatitude(), ll.getLongitude());
        _sender.SendMessage(JsonConvert.SerializeObject(new SatellitePosition
        {
            Public_PUI = "GroundPosition",
            MessageType = "IssPosition",
            Latitude = ll.getLatitude(),
            Longitude = ll.getLongitude(),
            Altitude = ll.getHeight(),
            TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        }));
    }

    private async Task<Tle> GetTLE()
    {
        var tleUrl = _config["TLE_URL"] ?? "https://www.celestrak.com/NORAD/elements/stations.txt";

        if ( !_tles.Tles.Any() || (_lastTleRefresh - DateTime.UtcNow) > TimeSpan.FromSeconds(_tleRefreshInterval))
        {
            _logger.LogInformation("Refreshing TLEs");
            _lastTleRefresh = DateTime.UtcNow;
            var client = new HttpClient();
            var response = await client.GetAsync(tleUrl);
            var data = await response.Content.ReadAsStreamAsync();
            _tles.Initialize(data);
        }
        return _tles.Tles[_config["TLE_NAME"]?? "25544"];
    }
}
