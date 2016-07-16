using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Casting;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CastingSample
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

        private CastingDevicePicker picker = null;
        private CastingConnection connection;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
            InitialCastingPicker();
            if (connection == null)
            {
                // 從按下的 button 出現 picker 内容
                Button btn = sender as Button;
                GeneralTransform transform = btn.TransformToVisual(Window.Current.Content as UIElement);
                Point pt = transform.TransformPoint(new Point(0, 0));
                picker.Show(new Rect(pt.X, pt.Y, btn.ActualWidth, btn.ActualHeight), Windows.UI.Popups.Placement.Above);
            }
            else
            {
                // 關掉現在的連綫，要記得去掉 event 注冊以免發生 memory leak
                connection.ErrorOccurred -= Connection_ErrorOccurred;
                connection.StateChanged -= Connection_StateChanged;
                await connection.DisconnectAsync();
                connection.Dispose();
                connection = null;
            }
        }

        private void InitialCastingPicker()
        {
            if (picker == null)
            {
                picker = new CastingDevicePicker();

                // 利用 MediaElement 的内容建立 filter
                picker.Filter.SupportedCastingSources.Add(player.GetAsCastingSource());

                // 注冊處理選擇設備的事件
                picker.CastingDeviceSelected += Picker_CastingDeviceSelected;

                // 注冊處理用戶沒有選擇設備的事件
                picker.CastingDevicePickerDismissed += Picker_CastingDevicePickerDismissed;

                // 設定 picker 要顯示的畫面内容 
                picker.Appearance.BackgroundColor = Colors.Black;
                picker.Appearance.ForegroundColor = Colors.White;
                picker.Appearance.AccentColor = Colors.Gray;

                picker.Appearance.SelectedAccentColor = Colors.Gray;

                picker.Appearance.SelectedForegroundColor = Colors.White;
                picker.Appearance.SelectedBackgroundColor = Colors.Black;
            }
        }

        private async void Picker_CastingDevicePickerDismissed(CastingDevicePicker sender, object args)
        {
            // 要使用 UI Thread 控制當用戶沒有選擇設備時候要繼續播放
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                player.Play();
            });
        }

        private async void Picker_CastingDeviceSelected(CastingDevicePicker sender, CastingDeviceSelectedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                connection = args.SelectedCastingDevice.CreateCastingConnection();
                //Hook up the casting events
                connection.ErrorOccurred += Connection_ErrorOccurred;
                connection.StateChanged += Connection_StateChanged;

                // Get the casting source from the MediaElement
                CastingSource source = null;

                try
                {
                    // Get the casting source from the Media Element
                    source = player.GetAsCastingSource();

                    // Start Casting
                    CastingConnectionErrorStatus status = await connection.RequestStartCastingAsync(source);

                    if (status == CastingConnectionErrorStatus.Succeeded)
                    {
                        player.Play();
                    }
                }
                catch
                {

                }
            });
        }

        #region Casting Connection Status Methods
        private async void Connection_StateChanged(CastingConnection sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (sender.State == CastingConnectionState.Connected ||
                    sender.State == CastingConnectionState.Rendering)
                {
                    castingIcon.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    castingIcon.Foreground = new SolidColorBrush(Colors.DimGray);
                }
            });
        }
        private async void Connection_ErrorOccurred(CastingConnection sender, CastingConnectionErrorOccurredEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // handle CastingConnectionErrorStatus
            });
        }

        #endregion

        private void OnGoToSenderPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DialSenderPage), null);
        }

        private void OnGoToReceiverPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DialReceiverPage), null);
        }
    }
}