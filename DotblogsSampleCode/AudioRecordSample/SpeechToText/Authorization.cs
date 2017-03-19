using System;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace AudioRecordSample
{
    public class Authorization
    {
        /// <summary>
        /// Bing Speech API subscription key
        /// </summary>
        private const string subscriptionKey = "";  // {your subscription key}

        /// <summary>
        /// Authenticate Uri
        /// </summary>
        const string Uri = "https://api.cognitive.microsoft.com/sts/v1.0";

        public static async Task<string> GetAccessToken()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"{Uri}/issueToken";

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                
                // use PORT method, and content length is 0
                var result = await client.PostAsync(new Uri(url), null);

                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}