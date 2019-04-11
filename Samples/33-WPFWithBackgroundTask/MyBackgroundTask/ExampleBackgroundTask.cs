using System;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace MyBackgroundTask
{
    public sealed class ExampleBackgroundTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral; 

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            // 利用 send toast 的方式表示有處理            
            string msg = $"收到 TimeZoneChanged 的事件，{TimeZoneInfo.Local.DisplayName}";

            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(msg));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(DateTime.Now.ToString()));

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);

            _deferral.Complete();
        }
    }
}
