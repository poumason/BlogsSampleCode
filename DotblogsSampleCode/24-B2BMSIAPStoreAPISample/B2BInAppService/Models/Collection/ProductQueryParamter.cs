using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BInAppService.Models.Collection
{
    /// <summary>
    /// Request body of the Query for products
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-us/windows/uwp/monetize/query-for-products"/>
    public class ProductQueryParamter
    {
        /// <summary>
        /// A UserIdentity object that represents the user being queried for products. For more information, see the table below.
        /// </summary>
        [JsonProperty("beneficiaries")]
        public List<UserIdentityData> Beneficiaries { get; set; } = new List<UserIdentityData>();

        /// <summary>
        /// <para>If specified, the service only returns products that match the specified product types.</para>
        /// <para>Supported product types are Application, Durable, and UnmanagedConsumable.</para>
        /// </summary>
        [JsonProperty("productTypes")]
        public List<string> ProductTypes { get; set; } = new List<string>();

        [JsonProperty("productSkuIds")]
        public List<ProductSkuData> ProductSkuIds { get; set; } = new List<ProductSkuData>();

        [JsonProperty("validityType")]
        public string ValityType { get { return "All"; } }
    }
}