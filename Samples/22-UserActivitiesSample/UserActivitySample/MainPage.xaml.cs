using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.UserActivities;
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

namespace UserActivitySample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainPageViewModel();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (string.IsNullOrEmpty(e.Parameter?.ToString()) == false)
            {
                string parameter = e.Parameter.ToString().Substring(1);

                Dictionary<string, string> keyValues = parameter.Split('&').ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);

                var activity = await UserActivityChannel.GetDefault().GetOrCreateUserActivityAsync(keyValues["id"]);

                if (activity.State== UserActivityState.Published)
                {
                    await UserActivityChannel.GetDefault().DeleteActivityAsync(keyValues["id"]);
                }
            }
        }
    }
}
