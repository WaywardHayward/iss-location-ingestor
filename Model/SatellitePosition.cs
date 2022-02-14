namespace iss_location_ingestor.Model
{
    public class SatellitePosition
    {
        public string MessageType { get; set; } = "IssPosition";

        public double Latitude {get;set;}

        public double Longitude {get;set;}

        public double Altitude {get;set;}
        public string Public_PUI { get; internal set; } = "GroundPosition";

        public string TimeStamp {get; internal set;}    = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

}