using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TitleBarSample
{
    public sealed partial class CustomTitleBar : UserControl
    {
        private CoreApplicationViewTitleBar coreTitleBar;

        public CustomTitleBar()
        {
            this.InitializeComponent();
            coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            this.Loaded += CustomTitleBar_Loaded;
            this.Unloaded += CustomTitleBar_Unloaded;
        }

        private void CustomTitleBar_Unloaded(object sender, RoutedEventArgs e)
        {
            coreTitleBar.IsVisibleChanged -= TitleBar_IsVisibleChanged;
            coreTitleBar.LayoutMetricsChanged -= TitleBar_LayoutMetricsChanged;
            coreTitleBar = null;
        }

        private void CustomTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.IsVisibleChanged -= TitleBar_IsVisibleChanged;
            coreTitleBar.IsVisibleChanged += TitleBar_IsVisibleChanged;
            coreTitleBar.LayoutMetricsChanged -= TitleBar_LayoutMetricsChanged;
            coreTitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (TitleBar != null)
            {
                TitleBar.Height = sender.Height;
            }
        }

        private void TitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (TitleBar != null)
            {
                TitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void SetContent(UIElement content)
        {
            rootGrid.Children.Add(content);
            Grid.SetRow((FrameworkElement)content, 1);

            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(TitleBar);
        }

        public void RemoveContent(UIElement content)
        {
            rootGrid.Children.Remove((FrameworkElement)content);
            coreTitleBar.ExtendViewIntoTitleBar = false;
            Window.Current.SetTitleBar(null);
        }
    }
}