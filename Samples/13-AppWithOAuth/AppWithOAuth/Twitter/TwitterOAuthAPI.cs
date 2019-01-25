using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace AppWithOAuth.Twitter
{
    public class TwitterOAuthAPI
    {
        private const String ConsumerKey = "";

        private const String ConsumerSecret = "";

        private static String CallbackUrl = "msft-3429cd1e811347f68e56340e3311b0b7://authorize";

        /// <summary>
        /// 請求取得 Request Token。
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetTwitterRequestTokenAsync()
        {
            string TwitterUrl = "https://api.twitter.com/oauth/request_token";

            string nonce = OAuthUtil.GetNonce();
            string timeStamp = OAuthUtil.GetTimeStamp();
            string SigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(CallbackUrl);
            SigBaseStringParams += "&" + "oauth_consumer_key=" + ConsumerKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            string SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);
            string Signature = OAuthUtil.GetSignature(SigBaseString, ConsumerSecret);

            TwitterUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature);
            HttpClient httpClient = new HttpClient();
            string GetResponse = await httpClient.GetStringAsync(new Uri(TwitterUrl));

            string request_token = null;
            string oauth_token_secret = null;
            string[] keyValPairs = GetResponse.Split('&');
            for (int i = 0; i < keyValPairs.Length; i++)
            {
                string[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }
            return request_token;
        }

        public static async Task<String> InvokeTwitterLogin()
        {
            try
            {
                // 1. get request token
                string oauth_token = await GetTwitterRequestTokenAsync();
                
                // 2. invoke user to login the twitter with get access token
                string TwitterUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauth_token;
                Uri StartUri = new Uri(TwitterUrl);
                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        StartUri,
                                                        new Uri(CallbackUrl));

                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var result = WebAuthenticationResult.ResponseData.ToString();
                    return result;
                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    return "HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString();
                }
                else
                {
                    return "Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString();
                }
            }
            catch (Exception ex)
            {
                //
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //
                return ex.Message;
            }
        }

        public static async Task<TwitterAccessToken> GetAccessToken(String token, String verifier)
        {
            String TwitterUrl = "https://api.twitter.com/oauth/access_token";

            string timeStamp = OAuthUtil.GetTimeStamp();
            string nonce = OAuthUtil.GetNonce();

            String SigBaseStringParams = "oauth_consumer_key=" + ConsumerKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            SigBaseStringParams += "&" + "oauth_token=" + token;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            String SigBaseString = "POST&";
            SigBaseString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);

            String Signature = OAuthUtil.GetSignature(SigBaseString, ConsumerSecret);

            HttpStringContent httpContent = new HttpStringContent("oauth_verifier=" + verifier, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            string authorizationHeaderParams = "oauth_consumer_key=\"" + ConsumerKey + "\", oauth_nonce=\"" + nonce +
                "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" + Uri.EscapeDataString(Signature) +
                "\", oauth_timestamp=\"" + timeStamp + "\", oauth_token=\"" + Uri.EscapeDataString(token) +
                "\", oauth_version=\"1.0\"";

            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", authorizationHeaderParams);
            var httpResponseMessage = await httpClient.PostAsync(new Uri(TwitterUrl), httpContent);
            string response = await httpResponseMessage.Content.ReadAsStringAsync();

            String[] Tokens = response.Split('&');
            string oauth_token_secret = null;
            string access_token = null;
            string screen_name = null;
            String user_id = null;
            TwitterAccessToken user = new TwitterAccessToken();
            for (int i = 0; i < Tokens.Length; i++)
            {
                String[] splits = Tokens[i].Split('=');
                switch (splits[0])
                {
                    case "screen_name":
                        screen_name = splits[1];
                        break;
                    case "user_id":
                        user_id = splits[1];
                        break;
                    case "oauth_token":
                        access_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }
            user.user_id = user_id;
            user.screen_name = screen_name;
            user.oauth_token = access_token;
            user.oauth_token_secret = oauth_token_secret;
            return user;
        }

        public static async Task<String> GetUserInfo(TwitterAccessToken accessToken, String verifier)
        {
            String result = String.Empty;
            try
            {
                string nonce = OAuthUtil.GetNonce();
                string timeStamp = OAuthUtil.GetTimeStamp();
                String url = "https://api.twitter.com/1.1/account/verify_credentials.json";

                // prepare base string parameters, include oauth_token, oauth_verifier
                var baseStringParams = new Dictionary<string, string>{
                                        {"oauth_consumer_key", ConsumerKey},
                                        {"oauth_nonce", nonce},
                                        {"oauth_signature_method", "HMAC-SHA1"},
                                        {"oauth_timestamp", timeStamp},
                                        {"oauth_token", accessToken.oauth_token},
                                        {"oauth_verifier", verifier},
                                        {"oauth_version", "1.0"} };

                string paramsBaseString = baseStringParams
                                           .OrderBy(kv => kv.Key)
                                           .Select(kv => kv.Key + "=" + kv.Value)
                                           .Aggregate((i, j) => i + "&" + j);

                string sigBaseString = "GET&";
                // signature base string uses base url
                sigBaseString += Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(paramsBaseString);

                // get signature by comsumer secret and oauth_token_secret
                string signature = OAuthUtil.GetSignature(sigBaseString, ConsumerSecret, accessToken.oauth_token_secret);

                // build header
                string data = "oauth_consumer_key=\"" + ConsumerKey +
                              "\", oauth_nonce=\"" + nonce +
                              "\", oauth_signature=\"" + Uri.EscapeDataString(signature) +
                              "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + timeStamp +
                              "\", oauth_token=\"" + accessToken.oauth_token +
                              "\", oauth_verifier=\"" + verifier +
                              "\", oauth_version=\"1.0\"";

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMsg = new HttpRequestMessage();
                requestMsg.Method = new HttpMethod("GET");
                requestMsg.RequestUri = new Uri(url);
                requestMsg.Headers.Authorization = new HttpCredentialsHeaderValue("OAuth", data);
                var response = await httpClient.SendRequestAsync(requestMsg);
                result = await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
    }
}
