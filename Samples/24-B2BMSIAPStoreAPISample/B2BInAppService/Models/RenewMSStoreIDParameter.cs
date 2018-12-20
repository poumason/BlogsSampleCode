using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BInAppService.Models
{
    public class RenewMSStoreIDParameter
    {
        [JsonProperty("serviceTicket")]
        public string ServiceTicket { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }
}