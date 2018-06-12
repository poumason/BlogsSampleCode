using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BInAppService.Models
{
    /// <summary>
    /// Create Azure AD access token
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-us/windows/uwp/monetize/view-and-grant-products-from-a-service#create-the-tokens"/>
    public class AzureADResponseData
    {
        public string token_type { get; set; }

        public string expires_in { get; set; }

        public string ext_expires_in { get; set; }

        public string expires_on { get; set; }

        public string not_before { get; set; }

        public string resource { get; set; }

        public string access_token { get; set; }
    }
}
