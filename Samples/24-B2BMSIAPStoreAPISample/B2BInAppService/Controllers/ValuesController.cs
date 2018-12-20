using B2BInAppService.Models;
using B2BInAppService.Models.Collection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace B2BInAppService.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public async Task<AccessTokenCollectionData> Get()
        {
            // 1. get header AAD access token
            AccessTokenCollectionData result = new AccessTokenCollectionData();
            result.Auth = await GetAzureADAccesToken(AuthType.Auth);
            result.Collection = await GetAzureADAccesToken(AuthType.Collection);
            result.Purchase = await GetAzureADAccesToken(AuthType.Purchase);

            return result;
        }

        // POST api/values
        [HttpPost]
        public async Task<string> Post([FromBody]PostActionData value)
        {
            var purchases = await GetSubscriptionsForUser(value.AuthData.Auth, value.PurchaseStoreID);
            var collection = await QueryOfProduct(value.AuthData.Auth, value.CollectionStoreID, value.UID);
            var renewCollection = await RenewMSStoreID(AuthType.Collection, value.AuthData.Auth, value.CollectionStoreID);
            var renewPurchase = await RenewMSStoreID(AuthType.Purchase, value.AuthData.Auth, value.PurchaseStoreID);

            string responseContent = $"{purchases}\r\n{collection}\r\n{renewCollection}\r\n{renewPurchase}";

            return responseContent;
        }

        private async Task<string> GetAzureADAccesToken(AuthType type)
        {
            string tenantId = "";
            string clientId = "";
            string clientSecret = "";

            string resource = "https://onestore.microsoft.com";

            switch (type)
            {
                case AuthType.Collection:
                    resource = "https://onestore.microsoft.com/b2b/keys/create/collections";
                    break;
                case AuthType.Purchase:
                    resource = "https://onestore.microsoft.com/b2b/keys/create/purchase";
                    break;
                default:
                    break;
            }

            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "resource", resource },
            });

            postContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync($"https://login.microsoftonline.com/{tenantId}/oauth2/token", postContent);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return string.Empty;
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var jsonObject = JsonConvert.DeserializeObject<AzureADResponseData>(responseContent);

                return jsonObject.access_token;
            }
        }

        /// <summary>
        /// Get subscriptions for a user
        /// </summary>
        /// <param name="accessToken">General AD Access Token</param>
        /// <param name="storeID">MS Store ID must be built by Purchase Access Token</param>
        /// <returns></returns>
        private async Task<string> GetSubscriptionsForUser(string accessToken, string storeID)
        {
            var purchase = new PurchaseQueryParameter
            {
                B2BKey = storeID
            };

            StringContent postContent = new StringContent(JsonConvert.SerializeObject(purchase));
            postContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Host = "purchase.mp.microsoft.com";

                var response = await client.PostAsync("https://purchase.mp.microsoft.com/v8.0/b2b/recurrences/query", postContent);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return string.Empty;
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
        }

        /// <summary>
        /// Query for products
        /// </summary>
        /// <param name="accessToken">General AD Access Token</param>
        /// <param name="storeID">MS Store ID must be built by Collection Access Token</param>
        /// <param name="uid"></param>
        /// <returns></returns>
        private async Task<string> QueryOfProduct(string accessToken, string storeID, string uid)
        {
            var collection = new ProductQueryParamter();
            collection.Beneficiaries.Add(new UserIdentityData
            {
                Reference = uid,
                Value = storeID
            });
            collection.ProductTypes.Add("Application");
            collection.ProductTypes.Add("Durable");
            collection.ProductTypes.Add("UnmanagedConsumable");
            //collection.ProductSkuIds.Add(new ProductSkuData
            //{
            //    ProductId = "",
            //    SkuID = ""
            //});

            StringContent content = new StringContent(JsonConvert.SerializeObject(collection));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Host = "collections.mp.microsoft.com";
                var response = await client.PostAsync(new Uri("https://collections.mp.microsoft.com/v6.0/collections/query"), content);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return string.Empty;
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
        }

        private async Task<string> RenewMSStoreID(AuthType type, string accessToken, string storeID)
        {
            if (type == AuthType.Auth)
            {
                return string.Empty;
            }

            RenewMSStoreIDParameter parameter = new RenewMSStoreIDParameter
            {
                ServiceTicket = accessToken,
                Key = storeID
            };

            StringContent postContent = new StringContent(JsonConvert.SerializeObject(parameter));
            postContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (HttpClient client = new HttpClient())
            {
                string url = string.Empty;

                if (type == AuthType.Collection)
                {
                    url = "https://collections.mp.microsoft.com/v6.0/b2b/keys/renew";
                    client.DefaultRequestHeaders.Host = "collections.mp.microsoft.com";
                }
                else
                {
                    url = "https://purchase.mp.microsoft.com/v6.0/b2b/keys/renew";
                    client.DefaultRequestHeaders.Host = "purchase.mp.microsoft.com";
                }

                var response = await client.PostAsync(url, postContent);

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
        }
    }
}