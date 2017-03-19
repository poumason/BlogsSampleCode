using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotWebhookSample.Models.LUIS
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