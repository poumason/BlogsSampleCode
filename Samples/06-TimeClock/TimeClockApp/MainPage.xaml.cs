using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TimeClockApp.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TimeClockApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel viewModel { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();

            viewModel = new MainPageViewModel(new DispatcherTimer());
            DataContext = viewModel;
            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            DisplayRequestManager.Instance.KeepOpenedScreen = false;
            viewModel.Stop();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayRequestManager.Instance.KeepOpenedScreen = true;
            viewModel.Start();
            HideStatusBar();

            //var api = new WeatherService();
            //api.UpdateToday();
        }

        private void HideStatusBar()
        {
            //Mobile customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    var task = statusBar.HideAsync();
                }
            }
        }
    }
}