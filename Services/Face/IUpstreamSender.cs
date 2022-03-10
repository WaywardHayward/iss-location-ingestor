
namespace iss_location_ingestor.Services.Face
{
    public interface IUpstreamSender
    {
        void SendMessage(string message);

        Task Flush();
    }
}