using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace OAuthBotAppSample.Google
{
    public class GoogleOAuthHelper
    {
        const string Google_clientId = "";
        const string Google_clientSecret = "";

        public static string GetGoogleLoginURL(ConversationReference conversationReference, string oauthCallback)
        {
            // 把 conversationreference 的内容放到 state 的參數裏面
            string stateToken = UrlToken.Encode(conversationReference);

            var uri = BotUtility.GetUri("https://accounts.google.com/o/oauth2/v2/auth",
                Tuple.Create("client_id", Google_clientId),
                Tuple.Create("redirect_uri", oauthCallback),
                Tuple.Create("response_type", "code"),
                Tuple.Create("access_type", "online"),
                Tuple.Create("scope", Uri.EscapeDataString("profile")),
                Tuple.Create("state", stateToken)
                );

            return uri.ToString();
        }

        public async static Task<GoogleAccessToken> ExchangeCodeForGoogleAccessToken(string code, string oauthCallback)
        {
            return await RequestAccessToken<GoogleAccessToken>("https://www.googleapis.com/oauth2/v4/token",
                Tuple.Create("client_id", Google_clientId),
                Tuple.Create("redirect_uri", oauthCallback),
                Tuple.Create("client_secret", Google_clientSecret),
                Tuple.Create("code", code),
                Tuple.Create("grant_type", "authorization_code")
                );
        }

        public static async Task<UserProfile> GetUserInfo(String token)
        {
            HttpClient tClient = new HttpClient();
            var rawData = await tClient.GetStringAsync(string.Format("https://www.googleapis.com/oauth2/v2/userinfo?access_token={0}", token));
            var result = JsonConvert.DeserializeObject<UserProfile>(rawData);
            return result;
        }

        private static async Task<T> RequestAccessToken<T>(string uri, params Tuple<string, string>[] queryParams)
        {
            string json;

            using (HttpClient client = new HttpClient())
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                foreach (var queryparam in queryParams)
                {
                    queryString[queryparam.Item1] = queryparam.Item2;
                }

                StringContent content = new StringContent(queryString.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await client.PostAsync(uri, content).ConfigureAwait(false);

                json = await response.Content.ReadAsStringAsync();
            }

            try
            {
                var result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Unable to deserialize the Facebook response.", ex);
            }
        }
    }
}