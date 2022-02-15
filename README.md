# ISS Location Ingestor
A worker app to calculate the Lat long position of ISS and send that to an event hub for processing.

## Settings and Configuration

| Setting | Description |
| --- | --- |
| `LLH_PROPAGATION_INTERVAL` | How offen to calculate the satellite's Lat Lon Altitude |
| `TLE_REFRESH_INTERVAL` | How offen to refresh the TLE data from `TLE_URL`|
| `TLE_URL` | The URL to fetch the TLE data from  defaults to https://www.celestrak.com/NORAD/elements/stations.txt |
| `TLE_BODY_NORAD_ID` | The NORAD ID of the object to generate LLH for defaults to 25544 (The ISS) |
| `EVENT_HUB_CONNECTION_STRING` | The connection string to the event hub to send messages to|
| `EVENT_HUB_NAME` | The name of the event hub to send messages to|

## Message Format

``` json
{
  "MessageType": <MessageType>,
  "Latitude": <Latitude>,
  "Longitude": <Longitude>,
  "Altitude": <Altitude>,
  "PUBLIC_PUI": "GroundPosition",
  "Timestamp": <Timestamp>,
  "NoradID": <NoradID>
}

