using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BInAppService.Models
{
    /// <summary>
    /// Server Azure AD access token collection.
    /// </summary>
    public class AccessTokenCollectionData
    {
        /// <summary>
        /// <para>General Azure AD Access token</para>
        /// <para>from https://onestore.microsoft.com</para>
        /// </summary>
        [JsonProperty("Auth")]
        public string Auth { get; set; }

        /// <summary>
        /// <para>Collection Azure AD Access token</para>
        /// <para>https://onestore.microsoft.com/b2b/keys/create/collections</para>
        /// </summary>
        [JsonProperty("Collection")]
        public string Collection { get; set; }

        /// <summary>
        /// <para>Purchsed Azure AD Access token</para>
        /// <para>https://onestore.microsoft.com/b2b/keys/create/purchase</para>
        /// </summary>
        [JsonProperty("Purchase")]
        public string Purchase { get; set; }
    }
}
