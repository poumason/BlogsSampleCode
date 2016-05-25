using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TitleBarSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ApplicationViewTitleBar titleBar;

        private CustomTitleBar customTitleBar;

        private UIElement cacheContent;

        public MainPage()
        {
            this.InitializeComponent();
            titleBar = ApplicationView.GetForCurrentView().TitleBar;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (UseStandardColors.IsChecked.Value)
            {
                // 要設定爲 null 才會還原是用 系統預設
                titleBar.BackgroundColor = null;
                titleBar.ForegroundColor = null;
                titleBar.InactiveBackgroundColor = null;
                titleBar.InactiveForegroundColor = null;

                titleBar.ButtonBackgroundColor = null;
                titleBar.ButtonHoverBackgroundColor = null;
                titleBar.ButtonPressedBackgroundColor = null;
                titleBar.ButtonInactiveBackgroundColor = null;

                titleBar.ButtonForegroundColor = null;
                titleBar.ButtonHoverForegroundColor = null;
                titleBar.ButtonPressedForegroundColor = null;
                titleBar.ButtonInactiveForegroundColor = null;
            }
            else if (UseCustomColor.IsChecked.Value)
            {
                // Title bar colors. Alpha must be 255.
                titleBar.BackgroundColor = new Color() { A = 255, R = 54, G = 60, B = 116 };
                titleBar.ForegroundColor = new Color() { A = 255, R = 232, G = 211, B = 162 };
                titleBar.InactiveBackgroundColor = new Color() { A = 255, R = 135, G = 141, B = 199 };
                titleBar.InactiveForegroundColor = new Color() { A = 255, R = 232, G = 211, B = 162 };

                // Title bar button background colors. Alpha is respected when the view is extended
                // into the title bar (see scenario 2). Otherwise, Alpha is ignored and treated as if it were 255.
                byte buttonAlpha = 255; /*(byte)(TransparentWhenExtended.IsChecked.Value ? 0 : 255);*/
                titleBar.ButtonBackgroundColor = new Color() { A = buttonAlpha, R = 54, G = 60, B = 116 };
                titleBar.ButtonHoverBackgroundColor = new Color() { A = buttonAlpha, R = 19, G = 21, B = 40 };
                titleBar.ButtonPressedBackgroundColor = new Color() { A = buttonAlpha, R = 232, G = 211, B = 162 };
                titleBar.ButtonInactiveBackgroundColor = new Color() { A = buttonAlpha, R = 135, G = 141, B = 199 };

                // Title bar button foreground colors. Alpha must be 255.
                titleBar.ButtonForegroundColor = new Color() { A = 255, R = 232, G = 211, B = 162 };
                titleBar.ButtonHoverForegroundColor = new Color() { A = 255, R = 255, G = 255, B = 255 };
                titleBar.ButtonPressedForegroundColor = new Color() { A = 255, R = 54, G = 60, B = 116 };
                titleBar.ButtonInactiveForegroundColor = new Color() { A = 255, R = 232, G = 211, B = 162 };
            }
            else
            {
                if (customTitleBar== null)
                {
                    customTitleBar = new CustomTitleBar();
                }

                if (UseExtenTitleBar.IsChecked.Value)
                {
                    // 先將目前畫面的内容暫時存起來
                    cacheContent = this.Content;
                    this.Content = customTitleBar;
                    // 設定給 custom title bar 并更新現在 mainpage 的 content
                    customTitleBar.SetContent(cacheContent);
                }
                else
                {
                    // 還原原本的 content 給 mainpage
                    this.Content = cacheContent;
                    // 移除 custom title bar 的 content
                    customTitleBar.RemoveContent(cacheContent);
                    cacheContent = null;
                }
            }
        }
    }
}