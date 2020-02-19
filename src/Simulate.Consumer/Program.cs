using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace Simulate.Consumer
{
    class Program
    {
        private const string HubName = "fishtanks";
        private const string HubConnectionString =
            "";
        private const string BlobConnectionString =
            "";
        private const string ProcessorLeaseContainerName = "temperature-processor-host";
        private static readonly string consumerGroupName = PartitionReceiver.DefaultConsumerGroupName;
        
        static async Task Main(string[] args)
        {
            
            var host = new EventProcessorHost(
                HubName, 
                consumerGroupName, 
                HubConnectionString, 
                BlobConnectionString, 
                ProcessorLeaseContainerName);

            await host.RegisterEventProcessorAsync<TemperatureEventProcessor>();
            Console.WriteLine("Event processor started, press enter to stop...");
            
            Console.ReadLine();
            await host.UnregisterEventProcessorAsync();
        }
    }
}