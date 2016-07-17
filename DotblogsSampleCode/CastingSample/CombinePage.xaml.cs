using ScreenCasting.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Casting;
using Windows.Media.DialProtocol;
using Windows.UI.Core;
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
    public sealed partial class CombinePage : Page
    {
        public CombinePage()
        {
            this.InitializeComponent();
        }

        DevicePicker devicePicker;
        DeviceInformation activeDevice;
        CastingConnection castingConnection;
        ProjectionBroker projectionInstance;

        private void OnCombinePickerClick(object sender, RoutedEventArgs e)
        {
            if (devicePicker== null)
            {
                devicePicker = new DevicePicker();

                // add casting
                devicePicker.Filter.SupportedDeviceSelectors.Add(CastingDevice.GetDeviceSelector(CastingPlaybackTypes.Video));

                // add dial
                devicePicker.Filter.SupportedDeviceSelectors.Add(DialDevice.GetDeviceSelector("castingsample"));

                // add projection
                devicePicker.Filter.SupportedDeviceSelectors.Add(ProjectionManager.GetDeviceSelector());

                devicePicker.DevicePickerDismissed += DevicePicker_DevicePickerDismissed;
                devicePicker.DeviceSelected += DevicePicker_DeviceSelected;
                devicePicker.DisconnectButtonClicked += DevicePicker_DisconnectButtonClicked;
            }

            player.Pause();

            // 從按下的 button 出現 picker 内容
            Button btn = sender as Button;
            GeneralTransform transform = btn.TransformToVisual(Window.Current.Content as UIElement);
            Point pt = transform.TransformPoint(new Point(0, 0));
            devicePicker.Show(new Rect(pt.X, pt.Y, btn.ActualWidth, btn.ActualHeight), Windows.UI.Popups.Placement.Above);
        }

        private async void DevicePicker_DisconnectButtonClicked(DevicePicker sender, DeviceDisconnectButtonClickedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async() =>
            {
                DeviceInformation selectedDevice = args.Device;
                if (await DialDevice.DeviceInfoSupportsDialAsync(selectedDevice))
                {
                    await StopDialConnection(sender, selectedDevice);
                    return;
                }

                await StopProjection(sender, selectedDevice);
            });
        }

        private async void DevicePicker_DeviceSelected(DevicePicker sender, DeviceSelectedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                DeviceInformation selectedDevice = args.SelectedDevice;

#if DEBUG
                // The args.SelectedCastingDevice is proxied from the picker process. The picker process is 
                // dismissmed as soon as you break into the debugger. Creating a non-proxied version 
                // allows debugging since the proxied version stops working once the picker is dismissed.
                selectedDevice = await DeviceInformation.CreateFromIdAsync(args.SelectedDevice.Id);
#endif

                if (await DialDevice.DeviceInfoSupportsDialAsync(selectedDevice))
                {
                    await SendDialParameter(sender, args);
                }
                else if (await CastingDevice.DeviceInfoSupportsCastingAsync(selectedDevice))
                {
                    await CastingVideoToScreen(sender, args);
                }
                else if (ProjectionManager.ProjectionDisplayAvailable)
                {
                    await ProjectioinViewToScreen(sender, args);
                }
            });
        }

        private async void DevicePicker_DevicePickerDismissed(DevicePicker sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                player.Play();
            });
        }

        #region DIAL 
        private async Task SendDialParameter(DevicePicker sender, DeviceSelectedEventArgs args)
        {
            try
            {
                // 設定遠端設備現在要準備連綫
                sender.SetDisplayStatus(args.SelectedDevice, "connecting", DevicePickerDisplayStatusOptions.ShowProgress);

                // 取得遠端設備中支援指定 app name 的 App              
                DialDevice dialDevice = await DialDevice.FromIdAsync(args.SelectedDevice.Id);
                DialApp app = dialDevice.GetDialApp("castingsample");

                if (app == null)
                {
                    // 嘗試建立 DIAL device，如果失敗代表那個設備不支援 DIAL
                    sender.SetDisplayStatus(args.SelectedDevice, "The app is not exist in the device.", DevicePickerDisplayStatusOptions.ShowRetryButton);
                }
                else
                {
                    // 請求送出參數到遠端設備的 App 
                    DialAppLaunchResult result = await app.RequestLaunchAsync("Test");

                    if (result == DialAppLaunchResult.Launched)
                    {
                        activeDevice = args.SelectedDevice;
                        sender.SetDisplayStatus(args.SelectedDevice, "connected", DevicePickerDisplayStatusOptions.ShowDisconnectButton);
                        sender.Hide();
                    }
                    else
                    {
                        sender.SetDisplayStatus(args.SelectedDevice, "Device Error", DevicePickerDisplayStatusOptions.ShowRetryButton);
                    }
                }
            }
            catch (Exception ex)
            {
                sender.SetDisplayStatus(args.SelectedDevice, ex.Message, DevicePickerDisplayStatusOptions.None);
            }
        }

        private async Task StopDialConnection(DevicePicker picker, DeviceInformation device)
        {
            try
            {
                // 取得被選擇的 dial device
                DialDevice selectedDialDevice = await DialDevice.FromIdAsync(device.Id);
                // 更新 picker status
                picker.SetDisplayStatus(device, "connecting", DevicePickerDisplayStatusOptions.ShowProgress);
                // 取得 dial app 
                DialApp app = selectedDialDevice.GetDialApp("castingsample");

                // 請求斷綫
                DialAppStopResult result = await app.StopAsync();

                if (result == DialAppStopResult.Stopped)
                {
                    picker.SetDisplayStatus(device, "Disconnected", DevicePickerDisplayStatusOptions.None);
                    activeDevice = null;
                    picker.Hide();
                }
                else
                {
                    if (result == DialAppStopResult.StopFailed || result == DialAppStopResult.NetworkFailure)
                    {
                        // 如果失敗的話要記得多 retry 的機制
                        picker.SetDisplayStatus(device, "Error", DevicePickerDisplayStatusOptions.ShowDisconnectButton);
                    }
                    else
                    {
                        // 如果設備沒有支援 Stop 機制，則直接清楚連綫就好
                        activeDevice = null;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        private async Task CastingVideoToScreen(DevicePicker sender, DeviceSelectedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                activeDevice = args.SelectedDevice;
                CastingDevice device =await  CastingDevice.FromIdAsync(args.SelectedDevice.Id);
                castingConnection = device.CreateCastingConnection();
                
                //Hook up the casting events
                //castingConnection.ErrorOccurred += Connection_ErrorOccurred;
                //castingConnection.StateChanged += Connection_StateChanged;

                // Get the casting source from the MediaElement
                CastingSource source = null;

                try
                {
                    // Get the casting source from the Media Element
                    source = player.GetAsCastingSource();

                    // Start Casting
                    CastingConnectionErrorStatus status = await castingConnection.RequestStartCastingAsync(source);

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

        #region Projection

        private async Task ProjectioinViewToScreen(DevicePicker sender, DeviceSelectedEventArgs args)
        {
            // 更新 picker 上設備的 status
            sender.SetDisplayStatus(args.SelectedDevice, "Connecting", DevicePickerDisplayStatusOptions.ShowProgress);

            // 取得目前選到設備的資訊
            activeDevice = args.SelectedDevice;

            // 現在 view 的 Id 與 CoreDispatcher
            int currentViewId = ApplicationView.GetForCurrentView().Id;
            CoreDispatcher currentDispatcher = Window.Current.Dispatcher;

            // 建立新的 view,
            if (projectionInstance.ProjectionViewPageControl == null)
            {
                await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // 建立新 viewe 的生命管理器
                    projectionInstance.ProjectionViewPageControl = ViewLifetimeControl.CreateForCurrentView();
                    projectionInstance.MainViewId = currentViewId;

                    var rootFrame = new Frame();
                    rootFrame.Navigate(typeof(ProjectionPage), projectionInstance);

                    // 這裏的 Window 代表是新建立這個 view 的 Window
                    // 但是要等到呼叫 ProjectionManager.StartProjectingAsync 才會顯示
                    Window.Current.Content = rootFrame;
                    Window.Current.Activate();
                });
            }

            // 直接切換到指定的 view id
            //bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(projectionInstance.ProjectionViewPageControl.Id);

            // 通知要使用新的 view
            projectionInstance.ProjectionViewPageControl.StartViewInUse();
            await ProjectionManager.SwapDisplaysForViewsAsync(projectionInstance.ProjectionViewPageControl.Id, currentViewId);
            try
            {
                await ProjectionManager.StartProjectingAsync(projectionInstance.ProjectionViewPageControl.Id, currentViewId, activeDevice);

                player.Pause();

                sender.SetDisplayStatus(args.SelectedDevice, "connected", DevicePickerDisplayStatusOptions.ShowDisconnectButton);
            }
            catch (Exception ex)
            {
                sender.SetDisplayStatus(args.SelectedDevice, ex.Message, DevicePickerDisplayStatusOptions.ShowRetryButton);
                if (ProjectionManager.ProjectionDisplayAvailable == false)
                {
                    throw;
                }
            }
        }

        private async Task StopProjection(DevicePicker sender, DeviceInformation device)
        {
            // 關閉新建立的 view
            projectionInstance.Content.StopProjection();

            //Update the display status for the selected device.
            sender.SetDisplayStatus(device, "Disconnecting", DevicePickerDisplayStatusOptions.ShowProgress);

            //Update the display status for the selected device.
            sender.SetDisplayStatus(device, "Disconnected", DevicePickerDisplayStatusOptions.None);

            // Set the active device variables to null
            activeDevice = null;
        }
        #endregion 
    }
}
