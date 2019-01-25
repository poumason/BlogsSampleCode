using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
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

namespace ServiceClient
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

        private async void OnInvokeLocalAppServiceClick(object sender, RoutedEventArgs e)
        {
            AppServiceConnection connection = new AppServiceConnection();
            connection.AppServiceName = "com.pou.MyApService";
            connection.PackageFamilyName = "f9842749-e4c8-4c15-bac8-bc018db1b2ea_s1mb6h805jdtj";

            var status = await connection.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                Debug.WriteLine("Failed to connect");
                return;
            }


            var message = new ValueSet();
            message.Add("cmd", "Query");
            message.Add("id", "1234");

            AppServiceResponse response = await connection.SendMessageAsync(message);
            string result = "";

            if (response.Status == AppServiceResponseStatus.Success)
            {
                if (response.Message["status"] as string == "OK")
                {
                    result = response.Message["name"] as string;
                }
            }

            Debug.WriteLine(result);
        }

        private async Task GetRemoteDevices()
        {

        }

        private async void OnInvokeRemoteAppServiceClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
