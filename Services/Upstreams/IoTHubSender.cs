using System.Text;
using iss_location_ingestor.Services.Face;
using Microsoft.Azure.Devices.Client;

namespace iss_location_ingestor.Services.Upstreams
{
    public class IoTHubSender : UpstreamSender, IUpstreamSender
    {
        private readonly ILogger<IoTHubSender> _logger;
        private readonly DeviceClient _deviceClient;

        public IoTHubSender(ILogger<IoTHubSender> logger, IConfiguration configuration,DeviceClient deviceClient) : base(logger, configuration)
        {
            _logger = logger;
            _deviceClient = deviceClient;
        }

        protected override async Task SendBatch()
        {
            try
            {
                var messages = new List<Message>();
                var batchId = Guid.NewGuid().ToString();

                _logger.LogInformation("Sending Batch: " + batchId);

                if (MessageQueue.Count == 0)
                {
                    _logger.LogInformation("No Messages for Batch: " + batchId);
                    return;
                }

                int messagesToDequeue = Math.Min(MessageQueue.Count, 100);

                lock (MessageQueue)
                {

                    for (int i = 0; i < messagesToDequeue; i++)
                    {
                        if (MessageQueue.Count == 0) break;
                        try
                        {
                            var message = MessageQueue.Dequeue();
                            if (message == null) continue;
                            messages.Add(new Message(Encoding.UTF8.GetBytes(message)));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Failed to dequeue message", ex.Message);
                        }
                    }

                }

                if(messages.Count > 0)
                    await _deviceClient.SendEventBatchAsync(messages);

                _logger.LogInformation("Sent Batch: " + batchId);
                UpdateLastSentTime();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}