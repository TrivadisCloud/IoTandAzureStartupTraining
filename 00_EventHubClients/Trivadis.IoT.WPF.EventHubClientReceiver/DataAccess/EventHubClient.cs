using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Trivadis.IoT.WPF.EventHubClientReceiver.Model;

namespace Trivadis.IoT.WPF.EventHubClientReceiver.DataAccess
{
  public class EventHubAccess
  {
    private List<Task> _tasks;
    private CancellationTokenSource _cts;
    private CancellationToken _token;

    public event EventHandler<SensorData> SensorDataReceived;


    public void StartReceivingMessages(string connectionString)
    {
      if (_cts != null)
      {
        _cts.Cancel();
      }

      var client = EventHubClient.CreateFromConnectionString(connectionString);

      EventHubConsumerGroup consumerGroup = client.GetDefaultConsumerGroup();
      string[] partitionIds = client.GetRuntimeInformation().PartitionIds;

      List<EventHubReceiver> receivers =
      partitionIds.Select(
        partitionId => consumerGroup.CreateReceiver(partitionId)).ToList();

      // Optionally pass in the DateTime from where you want to start reading the event stream
      //partitionIds.Select(
      //  partitionId => consumerGroup.CreateReceiver(partitionId,DateTime.UtcNow)).ToList();

      _cts = new CancellationTokenSource();
      _token = _cts.Token;

      _tasks = new List<Task>();
      foreach (var receiver in receivers)
      {
        var task = Task.Run(() =>
        {
          while (true)
          {
            try
            {
              if (_token.IsCancellationRequested)
              {
                break;
              }

              var message = receiver.Receive();

              if (message != null)
              {
                string body = Encoding.UTF8.GetString(message.GetBytes());
                var sensorData = JsonConvert.DeserializeObject<SensorData>(body);
                OnSensorDataReceived(sensorData);
              }
            }
            catch (Exception ex)
            {
              Debug.WriteLine(ex.Message);
            }
          }

        }, _token);
        _tasks.Add(task);
      }
    }

    protected virtual void OnSensorDataReceived(SensorData data)
    {
      SensorDataReceived?.Invoke(this, data);
    }
  }

  public class SensorDataReceivedEventArgs : EventArgs
  {
    public SensorDataReceivedEventArgs(SensorData sensorData, EventData data)
    {
      SensorData = sensorData;
      PartitionKey = data.PartitionKey;
    }

    public string PartitionKey { get; set; }

    public SensorData SensorData { get; set; }
  }

}
