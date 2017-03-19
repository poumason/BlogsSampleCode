using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotWebhookSample.Models.LUIS
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