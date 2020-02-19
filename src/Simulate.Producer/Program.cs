using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Telemetry.Models;

namespace Producers
{
    class Program
    {
        private const string DeviceConnectionString =
            "";
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniitalizing fish tank agent...");

            var device = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
            await device.OpenAsync();
            Console.WriteLine("Device is connected.");

            int totalMessageSent = 0;
            var random = new Random();
            while (true)
            {
                var temperature = new Temperature(random.Next(65, 90));
                await device.SendEventAsync(temperature.ToMessage());
                totalMessageSent++;
                Console.WriteLine($"Message sent to the cloud, temperature={temperature.Value}, count={temperature.Count}, total={totalMessageSent}");
                await Task.Delay(TimeSpan.FromMilliseconds(5000));
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}