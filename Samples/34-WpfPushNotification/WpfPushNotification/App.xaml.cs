using DesktopNotifications;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using static DesktopNotifications.NotificationActivator;

namespace WpfPushNotification
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Register AUMID and COM server (for Desktop Bridge apps, this no-ops)
            DesktopNotificationManagerCompat.RegisterAumidAndComServer<MyNotificationActivator>("{YOUR AUMID}");
            // Register COM server and activator type
            DesktopNotificationManagerCompat.RegisterActivator<MyNotificationActivator>();

            // If launched from a toast
            if (e.Args.Contains("-ToastActivated"))
            {
                // Our NotificationActivator code will run after this completes,
                // and will show a window if necessary.
            }

            else
            {
                // Show the window
                // In App.xaml, be sure to remove the StartupUri so that a window doesn't
                // get created by default, since we're creating windows ourselves (and sometimes we
                // don't want to create a window if handling a background activation).
                new MainWindow().Show();
            }

            base.OnStartup(e);
        }
    }

    // The GUID CLSID must be unique to your app. Create a new GUID if copying this code.
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(INotificationActivationCallback))]
    [Guid("461be5d7-9d43-4e6f-a3af-C173E6ADF0C3"), ComVisible(true)]
    public class MyNotificationActivator : NotificationActivator
    {
        public override void OnActivated(string invokedArgs, NotificationUserInput userInput, string appUserModelId)
        {
            // TODO: Handle activation

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Make sure we have a window open and in foreground
                OpenWindowIfNeeded();
                // And then show the image
                (App.Current.Windows[0] as MainWindow).SetMessage(invokedArgs);
            });
        }

        private void OpenWindowIfNeeded()
        {
            // Make sure we have a window open (in case user clicked toast while app closed)
            if (App.Current.Windows.Count == 0)
            {
                new MainWindow().Show();
            }

            // Activate the window, bringing it to focus
            App.Current.Windows[0].Activate();

            // And make sure to maximize the window too, in case it was currently minimized
            App.Current.Windows[0].WindowState = WindowState.Normal;
        }
    }
}
