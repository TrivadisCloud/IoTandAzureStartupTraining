using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivadis.IoT.Console.EventHubClientReceiver
{
  class Program
  {
    static void Main(string[] args)
    {
      var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
      System.Console.WriteLine("> Starting listening for events on event hub with the following connection string:");
      System.Console.WriteLine(connectionString);

      var client = EventHubClient.CreateFromConnectionString(connectionString);

      EventHubConsumerGroup consumerGroup = client.GetDefaultConsumerGroup();
      string[] partitionIds = client.GetRuntimeInformation().PartitionIds;

      List<EventHubReceiver> receivers = 
        partitionIds.Select(
          partitionId => consumerGroup.CreateReceiver(partitionId)).ToList();

      var tasks = new List<Task>();
      foreach (var receiver in receivers)
      {
        var task = Task.Run(() =>
         {
           string offset;
           while (true)
           {
             var message = receiver.Receive();
             if (message != null)
             {
               offset = message.Offset;
               string body = Encoding.UTF8.GetString(message.GetBytes());
               System.Console.WriteLine($"Received message offset: {offset} \nbody: {body}");
             }
           }
         }); 
        tasks.Add(task);
      }
      Task.WaitAll(tasks.ToArray());
    }
  }
}
