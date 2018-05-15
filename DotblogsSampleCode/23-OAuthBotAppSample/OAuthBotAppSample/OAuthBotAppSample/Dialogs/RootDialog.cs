using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using OAuthBotAppSample.Google;
using System.Collections.Generic;

namespace OAuthBotAppSample.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text == "login")
            {
                // 直接利用特定關鍵字觸發， 真實案例不適合這樣用
                await CheckLogin(context, activity);
            }
            else
            {
                // 接受來自 OAuthCallback ResumeAsync 回傳的内容 (例如： 使用 token 關鍵字， 真實案例不適合這樣用)
                if (activity.Text.StartsWith("token"))
                {
                    if (await HandleFromOAuthCallbackResponse(context, activity) == true)
                    {
                        return;
                    }
                }

                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                await context.PostAsync($"You sent {activity.Text} which was {length} characters");

                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task CheckLogin(IDialogContext context, IMessageActivity msg)
        {
            if (context.UserData.TryGetValue(BotUtility.AccessToken, out string token))
            {
                await context.PostAsync("you are already logined.");
            }
            else
            {
                // 保存這次對話的記錄，登入完畢後要 ResumeAsync 回到原本的對話
                var conversationReference = context.Activity.ToConversationReference();

                string authUrl = GoogleOAuthHelper.GetGoogleLoginURL(conversationReference, BotUtility.OAuthCallbackURL);

                var reply = context.MakeMessage();

                reply.Text = "Please login in using this card";
                reply.Attachments.Add(SigninCard.Create("You need to authorize me",
                                                        "Login to Google!",
                                                        authUrl
                                                        ).ToAttachment());
                await context.PostAsync(reply);
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task<bool> HandleFromOAuthCallbackResponse(IDialogContext context, IMessageActivity msg)
        {
            // 拿出 user data 裏面的資料
            if (context.UserData.TryGetValue(BotUtility.AccessToken, out string token) == false)
            {
                return false;
            }
            var userProfile = await GoogleOAuthHelper.GetUserInfo(token);
            var reply = context.MakeMessage();
            reply.Speak = reply.Text = $"Login success. your name is: {userProfile.Name}";
            reply.Attachments = new List<Attachment>() {
                                                new HeroCard("Login success",
                                                             $"your name is: {userProfile.Name}",
                                                             $"locale: {userProfile.Locale}",
                                                             new List<CardImage>()
                                                             {
                                                                 new CardImage() { Url = userProfile.Picture, Alt = "Hero Card Image Alt" }
                                                             }).ToAttachment()
                                            };
            await context.PostAsync(reply);
            return true;
        }
    }
}