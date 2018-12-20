using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using OAuthBotAppSample.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using OAuthBotAppSample.Google;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;

namespace OAuthBotAppSample
{
    [Route("api/OAuthCallback")]
    public class OAuthCallbackController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public void Get(string code)
        {

        }

        public async Task<HttpResponseMessage> Get(string code, string state, CancellationToken cancellationToken)
        {
            // 從 state 參數轉換為原本的 ConversationReference
            ConversationReference conversationReference = UrlToken.Decode<ConversationReference>(state);

            // 請求拿到 Google OAuth 的 Access Token
            var accessToken = await GoogleOAuthHelper.ExchangeCodeForGoogleAccessToken(code, BotUtility.OAuthCallbackURL);
          
            var msg = conversationReference.GetPostToBotMessage();

            // 取得目前談話對象的容器，並且把 UserData 加入 Access Token
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, msg))
            {
                IStateClient sc = scope.Resolve<IStateClient>();
                BotData userData = sc.BotState.GetUserData(msg.ChannelId, msg.From.Id);
                userData.SetProperty(BotUtility.AccessToken, accessToken.AccessToken);
                sc.BotState.SetUserData(msg.ChannelId, msg.From.Id, userData);
            }

            // 設定 ResumeAsync 回到 MessagesController 的識別值 (例如： 使用 token 關鍵字， 真實案例不適合這樣用)
            msg.Text = "token:" + accessToken.AccessToken;

            // 要記得使用 RsumeAsync 才能夠接回原本的 converstaion
            await Conversation.ResumeAsync(conversationReference, msg);

            return Request.CreateResponse("ok");
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}