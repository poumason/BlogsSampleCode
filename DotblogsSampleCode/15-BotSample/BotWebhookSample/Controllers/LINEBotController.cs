using BotWebhookSample.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BotWebhookSample.Controllers
{
    public class LINEBotController : ApiController
    {
        const string LINEChannel_Token = "";
        const string LINEChannel_Secret = "";

        [HttpPost]
        public IHttpActionResult POST()
        {
            string postData = string.Empty;

            try
            {
                // 1. get http post raw data(json)
                postData = Request.Content.ReadAsStringAsync().Result;

                // 2. parser LINE message
                var ReceivedMessage = isRock.LineBot.Utility.Parsing(postData);

                // 3. reply message      
                string Message;

                //Message = "你說了:" + ReceivedMessage.events[0].message.text;

                string keyword = ReceivedMessage.events[0].message.text;

                Message = QueryLUIS(keyword);

                //回覆用戶

                isRock.LineBot.Utility.ReplyMessage(ReceivedMessage.events[0].replyToken, Message, LINEChannel_Token);

                //回覆API OK

                //isRock.LineBot.Utility.ReplyStickerMessage(ReceivedMessage.events[0].replyToken, 2, 177, LINEChannel_Token);                

                return Ok();

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        private string QueryLUIS(string keyword)
        {
            try
            {
                MusicLuisService service = new MusicLuisService();

                return service.InvokeAPI(keyword);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}