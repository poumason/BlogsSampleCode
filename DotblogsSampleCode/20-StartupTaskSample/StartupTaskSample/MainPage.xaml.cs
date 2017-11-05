using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StartupTaskSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string payload = e.Parameter as string;

            if (!string.IsNullOrEmpty(payload))
            {
                tblState.Text = payload;
            }
        }

        private async void OnRequestStartupClick(object sender, RoutedEventArgs e)
        {
            // 取得目前 TaskId (MyStartupId_UWP, 根據設定在 package.appmanifest 的值) 的狀態
            var startupTask = await StartupTask.GetAsync("MyStartupId_UWP");
            
            if (startupTask.State == StartupTaskState.Disabled)
            {
                // 如果是 disabled 代表還沒有被加入到 Startup 裏面
                var newState = await startupTask.RequestEnableAsync();
                tblState.Text = newState.ToString();
            }
            else
            {
                tblState.Text = startupTask.State.ToString();
            }
        }
    }
}