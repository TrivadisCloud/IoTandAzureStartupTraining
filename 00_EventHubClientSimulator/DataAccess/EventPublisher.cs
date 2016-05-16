using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Trivadis.IoT.WPF.EventHubClientSimulator.Model;

namespace Trivadis.IoT.WPF.EventHubClientSimulator.DataAccess
{
  public class EventPublisher : IEventPublisher
  {
    public async Task<PublishResult> PublishAsync(string connectionString, SensorData sensorData)
    {
      try
      {
        EventHubClient client = EventHubClient.CreateFromConnectionString(connectionString);
        EventHubSender sender = client.CreateSender("MyDevice");

        string sensorDataJson = JsonConvert.SerializeObject(sensorData);

        var eventData = new EventData(Encoding.UTF8.GetBytes(sensorDataJson));

        await sender.SendAsync(eventData);
        return new PublishResult { IsSuccess = true };
      }
      catch(Exception ex)
      {
        return new PublishResult { Error = ex.Message };
      }
    }
  }

  public class PublishResult
  {
    public bool IsSuccess { get; set; }
    public string Error { get; set; }
  }
}
