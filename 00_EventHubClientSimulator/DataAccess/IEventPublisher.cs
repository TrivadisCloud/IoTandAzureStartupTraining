using System.Threading.Tasks;
using Trivadis.IoT.WPF.EventHubClientSimulator.Model;

namespace Trivadis.IoT.WPF.EventHubClientSimulator.DataAccess
{
  public interface IEventPublisher
  {
    Task<PublishResult> PublishAsync(string connectionString, SensorData sensorData);
  }
}
