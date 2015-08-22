using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace AppWithOAuth.Google
{
    public class GoogleSignInAPI
    {
        private static String clientId = "";

        private static String clientKey = "";

        private static String callbackUrl = "urn:ietf:wg:oauth:2.0:oob";

        private static String GetOAuthUrl(String id, String back)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("https://accounts.google.com/o/oauth2/auth?");
            builder.AppendFormat("client_id={0}", Uri.EscapeDataString(id));
            builder.Append("&response_type=code");
            // 宣告取得 openid, profile, email 三個内容。
            builder.AppendFormat("&scope={0}", Uri.EscapeDataString("openid profile email"));
            builder.Append("&state=foobar");
            builder.AppendFormat("&redirect_uri={0}", Uri.EscapeDataString(back));
            return builder.ToString();
        }

        public static async Task<String> RedirectGetAccessToken(String code)
        {
            Dictionary<String, String> param = new Dictionary<string, string>();
            param.Add("code", code);
            param.Add("client_id", clientId);
            param.Add("client_secret", clientKey);
            param.Add("redirect_uri", callbackUrl);
            param.Add("grant_type", "authorization_code");
            var stringContents = param.Select(s => s.Key + "=" + s.Value);
            String stringContent = String.Join("&", stringContents);
            HttpContent content = new StringContent(stringContent, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpClient httpClient = new HttpClient();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://www.googleapis.com/oauth2/v3/token");
            requestMessage.Content = content;
            requestMessage.Version = new Version("1.1");

            using (HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage))
            {
                return await responseMessage.Content.ReadAsStringAsync();
            }
        }

        public static async Task<String> InvokeGoogleSignIn()
        {
            try
            {
                String GoogleURL = GetOAuthUrl(clientId, callbackUrl);

                System.Uri StartUri = new Uri(GoogleURL);
                // When using the desktop flow, the success code is displayed in the html title of this end uri
                System.Uri EndUri = new Uri("https://accounts.google.com/o/oauth2/approval?");

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.UseTitle,
                                                        StartUri,
                                                        EndUri);

                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var result = WebAuthenticationResult.ResponseData.ToString();

                    if (result.Contains("Success"))
                    {
                        // 取得需要的 code
                        String code = result.Replace("Success ", "");
                        Int32 firstIdx = code.IndexOf("code=");
                        code = code.Substring(firstIdx + 5);
                        code = code.Substring(0, code.IndexOf("&"));
                        result = await RedirectGetAccessToken(code);
                    }
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

        public static async Task<UserProfile> GetUserInfo(String token)
        {
            HttpClient tClient = new HttpClient();
            var rawData = await tClient.GetStringAsync(string.Format("https://www.googleapis.com/oauth2/v2/userinfo?access_token={0}", token));
            return new UserProfile(rawData);
        }
    }
}