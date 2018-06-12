using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BInAppService.Models.Collection
{
    public class UserIdentityData
    {
        [JsonProperty("identityType")]
        public string Type
        {
            get { return "b2b"; }
        }

        [JsonProperty("identityValue")]
        public string Value { get; set; }

        /// <summary>
        /// <para>The requested identifier for the returned products. Returned items in the response body will have a matching localTicketReference.</para>
        /// <para>We recommend that you use the same value as the userId claim in the Microsoft Store ID key.</para>
        /// </summary>
        [JsonProperty("localTicketReference")]
        public string Reference { get; set; }
    }
}
