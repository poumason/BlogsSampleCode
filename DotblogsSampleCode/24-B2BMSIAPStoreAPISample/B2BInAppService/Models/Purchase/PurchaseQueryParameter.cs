using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BInAppService.Models
{
    /// <summary>
    /// <para>Request body of get subscription for user.</para>
    /// </summary>
    /// <see cref="https://docs.microsoft.com/zh-tw/windows/uwp/monetize/get-subscriptions-for-a-user"/>
    public class PurchaseQueryParameter
    {
        [JsonProperty("b2bKey")]
        public string B2BKey { get; set; }
    }
}
