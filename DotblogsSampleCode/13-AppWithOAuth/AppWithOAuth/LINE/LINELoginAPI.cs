using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using AppWithOAuth;

namespace AppWithOAuth.LINE
{
    public class LINELoginAPI
    {
        private const string ClientId = "";

        private const string ClientSecret = "";

        private const string CallbackUri = "https://localhost";


        public static async Task<string> RequestLoginAsync()
        {
            string state = "Windows_LINELoginAPI";
            string encodingCallback = Uri.EscapeDataString(CallbackUri);
            string oauthUrl = $"https://access.line.me/dialog/oauth/weblogin?response_type=code&client_id={ClientId}&redirect_uri={encodingCallback}&state={state}";

            string result = string.Empty;

            try
            {
                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(oauthUrl), new Uri(CallbackUri));

                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var response = WebAuthenticationResult.ResponseData.ToString();

                    // if your callback value is not URL, don't use it. please use split string methods.
                    Uri redirectUri = new Uri(response);
                    string query = redirectUri.Query.Substring(1);
                    Dictionary<string, string> keyValuePair = Utility.StringToDictionary(query);

                    if (keyValuePair.ContainsKey("error"))
                    {
                        // fail: http://sample.com/{Callback URL}?error=access_denied&state=[state]&errorCode=417&errorMessage=DISALLOWED
                        result = keyValuePair["errorMessage"];
                    }
                    else
                    {
                        // success: http://sample.com/callback?code=b5fd32eacc791df&state=123abc
                        string code = keyValuePair["code"];
                        string accessToken = await RedirectGetAccessTokenAsync(code);
                        result = accessToken;
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
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<string> RedirectGetAccessTokenAsync(string code)
        {
            string encodingCallback = Uri.EscapeDataString(CallbackUri);

            string url = "https://api.line.me/v1/oauth/accessToken";

            string postParameter = $"grant_type=authorization_code&client_id={ClientId}&client_secret={ClientSecret}&code={code}&redirect_uri={encodingCallback}";

            HttpStringContent httpContent = new HttpStringContent(postParameter, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            using (HttpClient httpClient = new HttpClient())
            {
                var httpResponseMessage = await httpClient.PostAsync(new Uri(url), httpContent);
                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }

        public static async Task GetProfile()
        {

        }
    }
}