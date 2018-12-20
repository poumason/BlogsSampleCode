using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BInAppService.Models.Collection
{
    public class ProductSkuData
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("skuId")]
        public string SkuID { get; set; }
    }
}
