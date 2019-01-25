using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.DialProtocol;
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
    public sealed partial class DialSenderPage : Page
    {
        public DialSenderPage()
        {
            this.InitializeComponent();
        }

        DialDevicePicker picker;
        DialDevice activeDialDevice;
        DeviceInformation activeDeviceInformation;

        private void InitDialDeivcePicker()
        {
            if (picker == null)
            {
                picker = new DialDevicePicker();
                // 設定支援的 app name
                picker.Filter.SupportedAppNames.Add("castingsample");
                picker.DialDevicePickerDismissed += Picker_DialDevicePickerDismissed;
                picker.DialDeviceSelected += Picker_DialDeviceSelected;
                picker.DisconnectButtonClicked += Picker_DisconnectButtonClicked;
            }
        }

        private async void Picker_DisconnectButtonClicked(DialDevicePicker sender, DialDisconnectButtonClickedEventArgs args)
        {
            // casting 必須在 UI Thread 下執行
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                try
                {                    
                    // 取得被選擇的 dial device
                    DialDevice selectedDialDevice = await DialDevice.FromIdAsync(args.Device.Id);
                    // 更新 picker status
                    picker.SetDisplayStatus(selectedDialDevice, DialDeviceDisplayStatus.Connecting);
                    // 取得 dial app 
                    DialApp app = selectedDialDevice.GetDialApp(txtAppName.Text);
                  
                    // 請求斷綫
                    DialAppStopResult result = await app.StopAsync();

                    if (result == DialAppStopResult.Stopped)
                    {
                        picker.SetDisplayStatus(args.Device, DialDeviceDisplayStatus.Disconnected);
                        activeDialDevice = null;
                        activeDeviceInformation = null;
                        picker.Hide();
                        tblMsg.Text += "Stoped, success";
                    }
                    else
                    {
                        if (result == DialAppStopResult.StopFailed || result == DialAppStopResult.NetworkFailure)
                        {
                            // 如果失敗的話要記得多 retry 的機制
                            picker.SetDisplayStatus(args.Device, DialDeviceDisplayStatus.Error);
                            tblMsg.Text += $"Stoped, {result}";
                        }
                        else
                        {
                            // 如果設備沒有支援 Stop 機制，則直接清楚連綫就好
                            activeDialDevice = null;
                            activeDeviceInformation = null;
                            tblMsg.Text += "the device does not support Stop";
                        }
                    }
                }
                catch (Exception ex)
                {
                    tblMsg.Text += ex.Message;
                }
            });
        }

        private async void Picker_DialDeviceSelected(DialDevicePicker sender, DialDeviceSelectedEventArgs args)
        {
            // casting 必須在 UI Thread 下執行
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    // 設定遠端設備現在要準備連綫
                    picker.SetDisplayStatus(args.SelectedDialDevice, DialDeviceDisplayStatus.Connecting);

                    // 取得遠端設備中支援指定 app name 的 App
                    DialApp app = args.SelectedDialDevice.GetDialApp(txtAppName.Text);

                    if (app == null)
                    {
                        // 嘗試建立 DIAL device，如果失敗代表那個設備不支援 DIAL
                        picker.SetDisplayStatus(args.SelectedDialDevice, DialDeviceDisplayStatus.Error);
                    }
                    else
                    {
                        // 請求送出參數到遠端設備的 App 
                        DialAppLaunchResult result = await app.RequestLaunchAsync(txtArgument.Text);
                        
                        if (result == DialAppLaunchResult.Launched)
                        {
                            activeDialDevice = args.SelectedDialDevice;
                            DeviceInformation selectedDeviceInformation = await DeviceInformation.CreateFromIdAsync(args.SelectedDialDevice.Id);

                            activeDeviceInformation = selectedDeviceInformation;
                            picker.SetDisplayStatus(activeDialDevice, DialDeviceDisplayStatus.Connected);
                            picker.Hide();
                            tblMsg.Text += "device connected";
                        }
                        else
                        {
                            picker.SetDisplayStatus(args.SelectedDialDevice, DialDeviceDisplayStatus.Error);
                            tblMsg.Text += "device error";
                        }
                    }
                }
                catch (Exception ex)
                {
                    tblMsg.Text += ex.Message;
                }
            });
        }

        private void Picker_DialDevicePickerDismissed(DialDevicePicker sender, object args)
        {
            // 處理如果用戶沒有選擇 device 的時候要幹嘛
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitDialDeivcePicker();
            // 從按下的 button 出現 picker 内容
            Button btn = sender as Button;
            GeneralTransform transform = btn.TransformToVisual(Window.Current.Content as UIElement);
            Point pt = transform.TransformPoint(new Point(0, 0));
            picker.Show(new Rect(pt.X, pt.Y, btn.ActualWidth, btn.ActualHeight), Windows.UI.Popups.Placement.Above);
        }
    }
}
