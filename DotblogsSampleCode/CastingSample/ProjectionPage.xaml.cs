using ScreenCasting.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CastingSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectionPage : Page
    {
        public ProjectionPage()
        {
            this.InitializeComponent();
        }

        private ProjectionBroker projectionBroker;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                Player.Source = new Uri("ms-appx:///Assets/Videos/kkbox_ja_adv.mp4");
                Player.Position = TimeSpan.FromSeconds(0);

                if (projectionBroker == null)
                {
                    projectionBroker = e.Parameter as ProjectionBroker;
                    projectionBroker.ProjectionViewPageControl.Released += LifetimeControl_Released;
                    projectionBroker.Content = this;
                }
            }
        }

        public async void StopProjection()
        {
            projectionBroker?.ProjectionViewPageControl?.StopViewInUse();
            await ProjectionManager.StopProjectingAsync(projectionBroker.ProjectionViewPageControl.Id, projectionBroker.MainViewId);
        }

        private void LifetimeControl_Released(object sender, EventArgs e)
        {
            Player.Stop();
            Player.Source = null;
            projectionBroker.Content = null;
            projectionBroker.ProjectionViewPageControl = null;
            Window.Current.Close();
        }
    }
}