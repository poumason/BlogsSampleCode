using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotWebhookSample.Models.LUIS
{
    public class EntityData
    {
        [JsonProperty("entity")]
        public string entity { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("startIndex")]
        public int StartIndex { get; set; }

        [JsonProperty("endIndex")]
        public int EndIndex { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("resolution")]
        public object Resolution { get; set; }
    }
}