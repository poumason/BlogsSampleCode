using Newtonsoft.Json;
using System.Collections.Generic;

namespace AudioRecordSample.LUISAPI
{
    public class TopScoringIntentData
    {
        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("actions")]
        public List<ActionData> Actions { get; set; }
    }
}