// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Allocation.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Telemetry.Models;

namespace Simulate.Consumer
{
    public class TemperatureEventProcessor : IEventProcessor
    {
        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"{nameof(TemperatureEventProcessor)} opened, processing partition: {context.PartitionId}");
            return Task.CompletedTask;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"{nameof(TemperatureEventProcessor)} closing, partition: {context.PartitionId}, reason: {reason}");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            Console.WriteLine($"Batch of events received on partition {context.PartitionId}");
            
            foreach (var eventData in messages)
            {
                Console.WriteLine($"Message received on device: {eventData.SystemProperties["iothub-connection-device-id"]}");
                
                var temperatures = BaseTelemetry.FromMessage<Temperature>(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                foreach (var temperature in temperatures)
                {
                    Console.WriteLine($"processed temperature event: value={temperature.Value}, count={temperature.Count}");    
                }
            }

            return context.CheckpointAsync();
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"{nameof(TemperatureEventProcessor)} error, partition: {context.PartitionId}, error: {error}");
            return Task.CompletedTask;
        }
    }
}