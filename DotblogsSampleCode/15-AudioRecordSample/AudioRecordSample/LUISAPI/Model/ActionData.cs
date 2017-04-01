using Newtonsoft.Json;
using System.Collections.Generic;

namespace AudioRecordSample.LUISAPI
{
    public class ActionData
    {
        [JsonProperty("triggered")]
        public bool Triggered { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parameters")]
        public List<ParameterData> Parameters { get; set; }
    }
}