using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotWebhookSample.Models.LUIS
{
    public class ValueData
    {
        public string entity { get; set; }

        public string type { get; set; }

        public object resolution { get; set; }
    }
}