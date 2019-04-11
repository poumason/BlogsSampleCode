using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;

namespace WpfPushNotification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            string guid = Guid.NewGuid().ToString();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                //channel.PushNotificationReceived += Channel_PushNotificationReceived;
                Debug.WriteLine(channel.Uri);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void Channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            switch (args.NotificationType)
            {
                case PushNotificationType.Raw:
                    break;
                case PushNotificationType.Tile:
                    break;
                case PushNotificationType.TileFlyout:
                    break;
                case PushNotificationType.Toast:
                    ShowCustomTaost(args.ToastNotification);
                    args.Cancel = true;
                    break;
            }
        }

        private void ShowCustomTaost(ToastNotification notification)
        {
            string title = "featured picture of the day";
            string content = notification.Content.InnerText;
            string image = "https://picsum.photos/360/180?image=104";
            string logo = "https://picsum.photos/64?image=883";

            string xmlString = $@"<toast><visual>
            <binding template='ToastGeneric'>
            <text>{title}</text>
            <text>{content}</text>
            <image src='{image}'/>
            <image src='{logo}' placement='appLogoOverride' hint-crop='circle'/>
            </binding>
            </visual></toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public void SetMessage(string message)
        {
            MessageTextBox.Text = message;
        }
    }
}