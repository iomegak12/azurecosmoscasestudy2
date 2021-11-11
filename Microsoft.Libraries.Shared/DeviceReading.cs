using Newtonsoft.Json;
using System;

namespace Microsoft.Libraries.Shared
{
    public class DeviceReading
    {
        [JsonProperty("id")]
        public string DeviceId { get; set; }
        public decimal DeviceTemperature { get; set; }
        public string DamageLevel { get; set; }
        public int DeviceAgeInDays { get; set; }
        public string Region { get; set; }
    }
}
