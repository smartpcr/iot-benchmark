using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace Telemetry.Models
{
    public class BaseTelemetry
    {
        public virtual Message ToMessage()
        {
            var json = JsonConvert.SerializeObject(this);
            return new Message(Encoding.ASCII.GetBytes(json));
        }

        public static List<T> FromMessage<T>(byte[] eventBody, int offset, int count) where T : BaseTelemetry
        {
            var json = Encoding.ASCII.GetString(eventBody, offset, count);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
    }
}