using JavaScriptExternal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace JavaScriptInvokeNativeSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private JavaScriptExternalObject javascriptExternalObject;

        public MainPage()
        {
            this.InitializeComponent();

            javascriptExternalObject = new JavaScriptExternalObject();
            javascriptExternalObject.FromJavaScriptMessage += JavascriptExternalObject_FromJavaScriptMessage;

            WebViewControl.NavigationCompleted += WebView_NavigationCompleted;
            WebViewControl.NavigationStarting += WebView_NavigationStarting;
            WebViewControl.ScriptNotify += WebView_ScriptNotify;
            WebViewControl.DOMContentLoaded += WebViewControl_DOMContentLoaded;
            
            WebViewControl.Source = new Uri("ms-appx-web:///html/TestPage.html");
        }

        private async void WebViewControl_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
        }

        private async void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // 定義公開的名稱為： external
            // JavaScriptExternalObject： 為 Javascript 裏面可以利用 external.{JavaScriptExternalObject 裏面的内容}
            sender.AddWebAllowedObject("external", javascriptExternalObject);           
        }

        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            string result = await WebViewControl.InvokeScriptAsync("eval", new string[] { "window.alert = function (AlertMessage) {window.external.notify(AlertMessage)}" });
        }

        private void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var result = e.Value;

            if (!string.IsNullOrEmpty(result))
            {
                JavascriptExternalObject_FromJavaScriptMessage(null, result);
            }
        }

        private async void JavascriptExternalObject_FromJavaScriptMessage(object sender, string e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var dialog = new MessageDialog(e);
                await dialog.ShowAsync();
            });
        }
    }
}