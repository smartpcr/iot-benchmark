// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Allocation.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text;
using System.Threading;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace Telemetry.Models
{
    public class Temperature : BaseTelemetry
    {
        private static long _count = 0;
        public double? Value { get; set; }
        public DateTime TimeStamp { get; }
        public long Count => _count;

        public Temperature(double? value)
        {
            TimeStamp = DateTime.UtcNow;
            Value = value;
            Interlocked.Increment(ref _count);
        }
    }
}