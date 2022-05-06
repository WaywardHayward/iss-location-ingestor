# ISS Location Ingestor
[![Build](https://github.com/WaywardHayward/iss-location-ingestor/actions/workflows/dotnet.yml/badge.svg)](https://github.com/WaywardHayward/iss-location-ingestor/actions/workflows/dotnet.yml)
A worker app to calculate the Lat long position of ISS and send that to an Azure Event Hub or an Azure IoT Hub for processing.

## Settings and Configuration

| Setting | Description |
| --- | --- |
| `BATCH_SEND_INTERVAL` | The interval in seconds to send the batch of events to the event hub if a batch has not been filled |
| `LLH_PROPAGATION_INTERVAL` | How offen to calculate the satellite's Lat Lon Altitude |
| `TLE_REFRESH_INTERVAL` | How offen to refresh the TLE data from `TLE_URL`|
| `TLE_URL` | The URL to fetch the TLE data from  defaults to https://www.celestrak.com/NORAD/elements/stations.txt |
| `TLE_BODY_NORAD_ID` | The NORAD ID of the object to generate LLH for defaults to 25544 (The ISS) |
| `UPSTREAM_CONNECTION_STRING` |  The connection string to the upstream data source, this can be used instead of IOT_HUB_CONNECTION_STRING or EVENT_HUB_CONNECTION string. |
| `UPSTREAM_MODE` | The mode of the upstream data source, set this to IOT_HUB to send to an IOT_HUB |
| `EVENT_HUB_CONNECTION_STRING` |  The connection string to the event hub. (obselete - use 'UPSTREAM_CONNECTION_STRING') |
| `IOT_HUB_CONNECTION_STRING` |  The connection string to the Iot hub. (obselete - use 'UPSTREAM_CONNECTION_STRING') |
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

