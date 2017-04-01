using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace AudioRecordSample.LUISAPI
{
    public class IntentData
    {
        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("actions")]
        public List<ActionData> Actions { get; set; }
    }
}