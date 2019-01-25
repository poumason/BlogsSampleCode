using System;
using System.Text;
using System.Web;
using Microsoft.Bot.Connector;

namespace OAuthBotAppSample
{
    public class BotUtility
    {
        public const string AccessToken = "ACCESS_TOKEN";

        public const string OAuthCallbackURL = "http://localhost:3979/api/OAuthCallback";

        public static Uri GetUri(string endPoint, params Tuple<string, string>[] queryParams)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var queryparam in queryParams)
            {
                queryString[queryparam.Item1] = queryparam.Item2;
            }

            var builder = new UriBuilder(endPoint);
            builder.Query = queryString.ToString();
            return builder.Uri;
        }

        public static string GetOAuthCallBack(ConversationReference conversationReference, string oauthCallback)
        {
            var uri = GetUri(oauthCallback,
                Tuple.Create("userId", TokenEncoder(conversationReference.User.Id)),
                Tuple.Create("botId", TokenEncoder(conversationReference.Bot.Id)),
                Tuple.Create("conversationId", TokenEncoder(conversationReference.Conversation.Id)),
                Tuple.Create("serviceUrl", TokenEncoder(conversationReference.ServiceUrl)),
                Tuple.Create("channelId", conversationReference.ChannelId)
                );
            return uri.ToString();
        }

        // because of a limitation on the characters in Facebook redirect_uri, we don't use the serialization of the cookie.
        // http://stackoverflow.com/questions/4386691/facebook-error-error-validating-verification-code
        private static string TokenEncoder(string token)
        {
            return HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(token));
        }
    }
}