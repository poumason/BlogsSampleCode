using Newtonsoft.Json;
using System.Collections.Generic;

namespace AudioRecordSample.LUISAPI
{
    public class ParameterData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("value")]
        public List<ValueData> Value { get; set; }
    }
}