using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotWebhookSample.Models.LUIS
{
    public class DialogData
    {
        [JsonProperty("contextId")]
        public string contextId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}