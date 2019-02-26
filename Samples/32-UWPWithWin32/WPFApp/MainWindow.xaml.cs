using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnSendToUWPClick(object sender, RoutedEventArgs e)
        {
            AppServiceConnection connection = new AppServiceConnection();
            connection.AppServiceName = "com.pou.MyAppService";
            connection.PackageFamilyName = "12023302-5f19-4075-a3d0-f3501877d795_s1mb6h805jdtj"; //Windows.ApplicationModel.Package.Current.Id.FamilyName;
            var result = await connection.OpenAsync();
            if (result == AppServiceConnectionStatus.Success)
            {
                ValueSet valueSet = new ValueSet();
                valueSet.Add("cmd", string.Empty);
                valueSet.Add("id", string.Empty);
                valueSet.Add("name", txtUserName.Text);

                var response = await connection.SendMessageAsync(valueSet);
                if (response.Status == AppServiceResponseStatus.Success)
                {
                    string responseMessage = response.Message["response"].ToString();
                    if (responseMessage == "success")
                    {
                        this.Hide();
                    }
                }
            }
        }
    }
}
