using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BInAppService.Models
{
    public class PostActionData
    {
        /// <summary>
        /// <para>From Collection Azure AD access token to build MS Store ID</para>
        /// </summary>
        public string CollectionStoreID { get; set; }

        /// <summary>
        /// <para>From Purchased Azure AD access token to build MS Store ID</para>
        /// </summary>
        public string PurchaseStoreID { get; set; }

        /// <summary>
        /// <para>publisher id</para>
        /// </summary>
        public string UID { get; set; }

        public AccessTokenCollectionData AuthData { get; set; }
    }
}
