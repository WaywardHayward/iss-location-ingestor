
using One_Sgp4;

namespace iss_location_ingestor.Services
{
    public class TleCache
    {
        private ILogger<TleCache> _logger;

        public Dictionary<string, Tle> Tles { get; set; }

        public TleCache(ILogger<TleCache> logger)
        {
            _logger = logger;
            Tles = new Dictionary<string, Tle>();
        }

        public void Initialize(Stream data)
        {
            lock (Tles)
            {
                Tles.Clear();
                var text = new StreamReader(data).ReadToEnd();
                var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < lines.Length; i += 3)
                {
                    try
                    {
                        var line0 = lines[i];
                        var line1 = lines[i + 1];
                        var line2 = lines[i + 2];

                        var tle = ParserTLE.parseTle(line1, line2, line0);

                        if (!Tles.ContainsKey(tle.getNoradID())){
                            _logger.LogInformation($"Adding TLE: {tle.getName()}-{tle.getNoradID()}");
                            Tles.Add(tle.getNoradID(), tle);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }


            }
        }
    }
}