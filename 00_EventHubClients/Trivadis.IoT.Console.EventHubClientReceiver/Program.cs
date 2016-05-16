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
      var consumerGroup = client.GetDefaultConsumerGroup();
      var receiver = consumerGroup.CreateReceiver(client.GetRuntimeInformation().PartitionIds[0]);

      bool receive = true;
      string offset;
      while (receive)
      {
        var message = receiver.Receive();
        offset = message.Offset;
        string body = Encoding.UTF8.GetString(message.GetBytes());
        System.Console.WriteLine(String.Format("Received message offset: {0} \nbody: {1}", offset, body));
      }
    }
  }
}
