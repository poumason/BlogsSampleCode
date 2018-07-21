using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace MyAppService
{
    public sealed class ServiceTask : IBackgroundTask
    {
        private BackgroundTaskDeferral backgroundTaskDeferral;
        private AppServiceConnection appServiceconnection;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Background Task 被建立時，取得 deferral 拉長生命周期，避免被結束
            this.backgroundTaskDeferral = taskInstance.GetDeferral();

            // 一定要註冊處理 Canceled 事件來正確釋放用到的資源
            taskInstance.Canceled += OnTaskCanceled;

            // 根據被啓動的 Instance 類型，建立 App Service Connection，並註冊 Request 事件.
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            appServiceconnection = details.AppServiceConnection;
            appServiceconnection.RequestReceived += OnRequestReceived;
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (this.backgroundTaskDeferral != null)
            {
                // Complete the service deferral.
                this.backgroundTaskDeferral.Complete();
            }
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            // 當 App Service 收到請求時，該 method 就會被觸發

            // 先要求取得 取得 deferral 拉長生命周期
            var requestDeferral = args.GetDeferral();

            ValueSet message = args.Request.Message;

            string cmd = message["cmd"] as string;
            string id = message["id"] as string;

            ValueSet responseMsg = new ValueSet();

            switch (cmd)
            {
                case "Query":
                    responseMsg.Add("id", "123456");
                    responseMsg.Add("name", "pou");
                    responseMsg.Add("status", "OK");
                    var result = await args.Request.SendResponseAsync(responseMsg);
                    break;
            }

            requestDeferral.Complete();
        }

    }
}
