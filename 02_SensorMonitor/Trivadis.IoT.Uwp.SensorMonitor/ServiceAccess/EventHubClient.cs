using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Trivadis.IoT.Uwp.SensorMonitor.ServiceAccess
{
  /// <summary>
  /// NOTE: This class can be generated including the Shared Access Signature and your EventHubRest-Uri
  ///       by using the "Event Hub Rest Client Generator", which is free and open source. Download it here:
  ///       https://github.com/thomasclaudiushuber/EventHub.RestClientGenerator/releases
  /// </summary>
  public class EventHubClient
  {
    const string SharedAccessSignature = "[TODO: YOUR Shared Access Generator goes here]";
    const string EventHubRestUri = "https://youreventhubnamespace.servicebus.windows.net/youreventhub/messages";
    public async Task<bool> PostDataAsync(string jsonObjectToSend)
    {
      try
      {
        var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", SharedAccessSignature);

        var content = new StringContent(jsonObjectToSend, Encoding.UTF8, "application/json");
        content.Headers.Add("ContentType", "application/json");

        var result = await httpClient.PostAsync(EventHubRestUri, content);

        return result.IsSuccessStatusCode;
      }
      catch
      {
        return false;
      }
    }
  }
}
