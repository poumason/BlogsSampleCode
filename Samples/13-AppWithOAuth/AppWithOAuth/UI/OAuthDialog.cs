using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace AppWithOAuth.UI
{
    public sealed class OAuthDialog : Control
    {
        /// <summary>
        /// handle Redirect Uri (ms-appx:// or other) events
        /// </summary>
        public event EventHandler<string> AuthorizeRedirectChanged;

        public static readonly DependencyProperty AuthorizeUrlProperty =
            DependencyProperty.Register("AuthorizeUrl", typeof(string), typeof(OAuthDialog), new PropertyMetadata(string.Empty));
        public string AuthorizeUrl
        {
            get { return (string)GetValue(AuthorizeUrlProperty); }
            set { SetValue(AuthorizeUrlProperty, value); }
        }

        private WebView WebViewControl { get; set; }

        private Grid RootContainer { get; set; }

        private Grid ContentContainer { get; set; }

        private Button HideButton { get; set; }

        private Border BackgroundBorder { get; set; }

        private ProgressRing ProgressRingControl { get; set; }

        private Popup PopupControl { get; set; }

        public OAuthDialog()
        {
            this.DefaultStyleKey = typeof(OAuthDialog);

            PopupControl = new Popup();
            PopupControl.Child = this;

            if (Window.Current != null)
            {
                var currentWindow = Window.Current;
                currentWindow.SizeChanged += CurrentWindow_SizeChanged;
            }

            SystemNavigationManager.GetForCurrentView().BackRequested -= AuthorizeDialogControl_BackRequested;
            SystemNavigationManager.GetForCurrentView().BackRequested += AuthorizeDialogControl_BackRequested;

            Unloaded += AuthorizeDialogControl_Unloaded;
        }

        private void AuthorizeDialogControl_Unloaded(object sender, RoutedEventArgs e)
        {
            UnSubscribeEvetns();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RootContainer = GetTemplateChild("RootContainer") as Grid;
            ContentContainer = GetTemplateChild("ContentContainer") as Grid;

            WebViewControl = GetTemplateChild("WebViewControl") as WebView;

            if (WebViewControl != null)
            {
                WebViewControl.NavigationStarting -= WebViewControl_NavigationStarting;
                WebViewControl.NavigationStarting += WebViewControl_NavigationStarting;
                WebViewControl.NavigationCompleted -= WebViewControl_NavigationCompleted;
                WebViewControl.NavigationCompleted += WebViewControl_NavigationCompleted;
                WebViewControl.NavigationFailed -= WebViewControl_NavigationFailed;
                WebViewControl.NavigationFailed += WebViewControl_NavigationFailed;
                WebViewControl.UnsupportedUriSchemeIdentified -= WebViewControl_UnsupportedUriSchemeIdentified;
                WebViewControl.UnsupportedUriSchemeIdentified += WebViewControl_UnsupportedUriSchemeIdentified;

                if (Uri.TryCreate(AuthorizeUrl, UriKind.Absolute, out var authorizeUri))
                {
                    WebViewControl.Navigate(authorizeUri);
                }
            }

            HideButton = GetTemplateChild("HideButton") as Button;

            if (HideButton != null)
            {
                HideButton.Click += HideButton_Click;
            }

            BackgroundBorder = GetTemplateChild("BackgroundBorder") as Border;

            ProgressRingControl = GetTemplateChild("ProgressRingControl") as ProgressRing;

            RefreshrLayout();
        }

        private void CurrentWindow_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            RefreshrLayout();
        }

        private void WebViewControl_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            ControlProgressRingControl(false);
        }

        private void WebViewControl_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            ControlProgressRingControl(false);
        }

        private void WebViewControl_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            ControlProgressRingControl(true);
        }

        private void WebViewControl_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {
            // handle custome scheme in the redirect uri, such as： ms-app://{store id}
            AuthorizeRedirectChanged?.Invoke(sender, args.Uri.OriginalString);
            args.Handled = true;
            Hide();
        }

        private void AuthorizeDialogControl_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (PopupControl.IsOpen)
            {
                Hide();
                e.Handled = true;
            }
        }

        private void HideButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        public void Show()
        {
            PopupControl.IsOpen = true;
        }

        public void Hide()
        {
            PopupControl.IsOpen = false;
            ClearCookie();
        }

        private void RefreshrLayout()
        {
            try
            {
                Window.Current.Content.UpdateLayout();
            }
            catch (Exception)
            {
            }

            RootContainer.Width = Window.Current.Bounds.Width;
            RootContainer.Height = Window.Current.Bounds.Height;
            ContentContainer.Height = Window.Current.Bounds.Height * 0.7;
        }

        private void ControlProgressRingControl(bool isActive)
        {
            if (ProgressRingControl != null)
            {
                ProgressRingControl.IsActive = isActive;
            }
        }

        private void ClearCookie()
        {
            if (Uri.TryCreate(AuthorizeUrl, UriKind.Absolute, out var authUri))
            {
                // capture the domain of the OAuth URL, then clear all cookies
                string targetUrl = $"{authUri.Scheme}://{authUri.Host}";

                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                HttpCookieManager manager = filter.CookieManager;
                HttpCookieCollection collection = manager.GetCookies(new Uri(targetUrl));

                foreach (var item in collection)
                {
                    manager.DeleteCookie(item);
                }
            }
        }

        private void UnSubscribeEvetns()
        {
            if (WebViewControl != null)
            {
                WebViewControl.NavigationStarting -= WebViewControl_NavigationStarting;
                WebViewControl.NavigationCompleted -= WebViewControl_NavigationCompleted;
                WebViewControl.NavigationFailed -= WebViewControl_NavigationFailed;
                WebViewControl.UnsupportedUriSchemeIdentified -= WebViewControl_UnsupportedUriSchemeIdentified;
            }

            if (HideButton != null)
            {
                HideButton.Click -= HideButton_Click;
            }

            if (Window.Current != null)
            {
                var currentWindow = Window.Current;
                currentWindow.SizeChanged -= CurrentWindow_SizeChanged;
            }

            SystemNavigationManager.GetForCurrentView().BackRequested -= AuthorizeDialogControl_BackRequested;
        }
    }
}
