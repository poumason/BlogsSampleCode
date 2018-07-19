using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.System.RemoteSystems;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RemoteSystemSample
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<RemoteSystem> SystemList => devicesList;
        private ObservableCollection<RemoteSystem> devicesList;

        public ObservableCollection<RemoteSystemSessionInfo> AvaiableSessionsList => sessionList;
        private ObservableCollection<RemoteSystemSessionInfo> sessionList;

        private string log;
        public string Log
        {
            get { return log; }
            set
            {
                log = value;
                RaisePropertyChanged("Log");
            }
        }

        private RemoteSystemWatcher devicesWatcher;
        private RemoteSystemSessionWatcher sessionWatcher;

        private CoreDispatcher dispatcher;

        private RemoteSystem currentRemoteSystem;

        private RemoteSystemSessionController sessionController;
        private RemoteSystemSession currentSession;
        private RemoteSystemSessionMessageChannel appMessageChannel;

        public MainPageViewModel(CoreDispatcher dispatcher)
        {
            devicesList = new ObservableCollection<RemoteSystem>();
            this.dispatcher = dispatcher;
        }

        #region Discover Devices
        public async void OnStartDiscoverClick(object sender, RoutedEventArgs e)
        {
            // 要使用 RemoteSystem 前，需要先要求用戶給與權限
            var requestPermission = await RemoteSystem.RequestAccessAsync();

            if (requestPermission != RemoteSystemAccessStatus.Allowed)
            {
                return;
            }

            OnStopDiscoverClick(sender, e);
            DiscoverDevicesAsync();
        }

        public void OnStopDiscoverClick(object sender, RoutedEventArgs e)
        {
            if (devicesWatcher == null)
            {
                return;
            }

            devicesWatcher.Stop();
            devicesWatcher.RemoteSystemAdded -= DevicesWatcher_RemoteSystemAdded;
            devicesWatcher.RemoteSystemRemoved -= DevicesWatcher_RemoteSystemRemoved;
            devicesWatcher.RemoteSystemUpdated -= DevicesWatcher_RemoteSystemUpdated;
            devicesWatcher.ErrorOccurred -= DevicesWatcher_ErrorOccurred;
            devicesWatcher = null;
        }

        private List<IRemoteSystemFilter> GetRemoteSystemFilter()
        {
            List<IRemoteSystemFilter> filters = new List<IRemoteSystemFilter>();

            // 設定要用什麽方式找設備, 利用 Any 比較多設備可以被找到
            filters.Add(new RemoteSystemDiscoveryTypeFilter(RemoteSystemDiscoveryType.Any));

            // 設定找到的設備要是什麽狀態
            filters.Add(new RemoteSystemStatusTypeFilter(RemoteSystemStatusType.Available));

            // 設定要找尋的設備類型
            //filters.Add(new RemoteSystemKindFilter(new List<string>
            //{
            //    RemoteSystemKinds.Desktop, RemoteSystemKinds.Laptop, RemoteSystemKinds.Tablet,
            //    RemoteSystemKinds.Phone,
            //    RemoteSystemKinds.Xbox
            //}));

            // 設定是否需要驗證的設備, 如果要相同帳號可以選 SameUser，預設是 SameUser
            //filters.Add(new RemoteSystemAuthorizationKindFilter(RemoteSystemAuthorizationKind.Anonymous));

            return filters;
        }

        private void DiscoverDevicesAsync()
        {
            var filters = GetRemoteSystemFilter();

            // Filters 需要在建立 RemoteSystemWatcher 建構子一起傳入
            devicesWatcher = RemoteSystem.CreateWatcher(filters);
            devicesWatcher.RemoteSystemAdded += DevicesWatcher_RemoteSystemAdded;
            devicesWatcher.RemoteSystemRemoved += DevicesWatcher_RemoteSystemRemoved;
            devicesWatcher.RemoteSystemUpdated += DevicesWatcher_RemoteSystemUpdated;
            //devicesWatcher.ErrorOccurred += DevicesWatcher_ErrorOccurred;

            devicesWatcher.Start();
        }

        private void DevicesWatcher_ErrorOccurred(RemoteSystemWatcher sender, RemoteSystemWatcherErrorOccurredEventArgs args)
        {
            Debug.WriteLine(args.Error);
        }

        private void DevicesWatcher_RemoteSystemUpdated(RemoteSystemWatcher sender, RemoteSystemUpdatedEventArgs args)
        {
            var updateTask = this.dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var existDevice = devicesList.Where(x => x.Id == args.RemoteSystem.Id).FirstOrDefault();

                if (existDevice != null)
                {
                    devicesList.Remove(existDevice);
                }

                devicesList.Add(args.RemoteSystem);
            });
        }

        private void DevicesWatcher_RemoteSystemRemoved(RemoteSystemWatcher sender, RemoteSystemRemovedEventArgs args)
        {
            var updateTask = dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var existDevice = devicesList.Where(x => x.Id == args.RemoteSystemId).FirstOrDefault();

                if (existDevice != null)
                {
                    devicesList.Remove(existDevice);
                }
            });
        }

        private void DevicesWatcher_RemoteSystemAdded(RemoteSystemWatcher sender, RemoteSystemAddedEventArgs args)
        {
            var updateTask = this.dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                devicesList.Add(args.RemoteSystem);
            });
        }
        #endregion

        #region Discover_Action

        public void OnRemoteSystemsListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            currentRemoteSystem = listView?.SelectedItem as RemoteSystem;
        }

        public async void OnInvokeRemoteSystemByUriAsync(object sender, RoutedEventArgs e)
        {
            if (currentRemoteSystem == null)
            {
                return;
            }

            // 檢查設備是否支援需要的特性
            //await currentRemoteSystem.GetCapabilitySupportedAsync(KnownRemoteSystemCapabilities.LaunchUri)
            var launchRequest = new RemoteSystemConnectionRequest(currentRemoteSystem);
            var result = await RemoteLauncher.LaunchUriAsync(launchRequest, new Uri("https://poumason.blogspot.com"));
            Debug.WriteLine(result.ToString());
        }

        public async void OnInvokeRemoteSystemByAppServiceAsync(object sender, RoutedEventArgs e)
        {
            if (currentRemoteSystem == null)
            {
                return;
            }

            // 檢查設備是否支援需要的特性
            if (await currentRemoteSystem.GetCapabilitySupportedAsync(KnownRemoteSystemCapabilities.AppService))
            {
                AppServiceConnection connection = new AppServiceConnection();
                connection.AppServiceName = "com.pou.MyApService";
                connection.PackageFamilyName = "f9842749-e4c8-4c15-bac8-bc018db1b2ea_s1mb6h805jdtj";

                RemoteSystemConnectionRequest appServiceRequest = new RemoteSystemConnectionRequest(currentRemoteSystem);

                AppServiceConnectionStatus status = await connection.OpenRemoteAsync(appServiceRequest);

                if (status == AppServiceConnectionStatus.Success)
                {
                    var message = new ValueSet();
                    message.Add("cmd", "Query");
                    message.Add("id", "1234");

                    AppServiceResponse response = await connection.SendMessageAsync(message);

                    if (response.Status == AppServiceResponseStatus.Success)
                    {
                        if (response.Message["status"] as string == "OK")
                        {
                            Debug.WriteLine(response.Message["name"] as string);
                        }
                    }
                }
            }
        }
        #endregion

        #region RemoteSystemSession

        public async void OnCreateRemoteSystemSessionClick(object sender, RoutedEventArgs e)
        {
            if (sessionController != null)
            {
                return;
            }

            // 加入 option 限制只有被邀請的人才可以加入
            //RemoteSystemSessionOptions options = new RemoteSystemSessionOptions()
            //{
            //    IsInviteOnly = true
            //};

            sessionController = new RemoteSystemSessionController("today is happy day");

            sessionController.JoinRequested += SessionController_JoinRequested;

            RemoteSystemSessionCreationResult result = await sessionController.CreateSessionAsync();

            if (result.Status == RemoteSystemSessionCreationStatus.Success)
            {
                currentSession = result.Session;
                currentSession.Disconnected += (obj, args) =>
                {
                    Debug.WriteLine($"session_disconnected: {args.Reason.ToString()}");
                };

                RegistMessageChannel();

                if (currentRemoteSystem != null)
                {
                    try
                    {
                        var inviationResult = await currentSession.SendInvitationAsync(currentRemoteSystem);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                Log = currentSession.DisplayName;
            }
        }

        private async void SessionController_JoinRequested(RemoteSystemSessionController sender, RemoteSystemSessionJoinRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();

            var remoteSystem = args.JoinRequest.Participant.RemoteSystem;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var dialog = new MessageDialog($"do you access {remoteSystem.DisplayName} to join the session?");
                dialog.Commands.Add(new UICommand("Accept", (cmd) =>
                {
                    args.JoinRequest.Accept();
                }));
                dialog.Commands.Add(new UICommand("Abort"));
                dialog.DefaultCommandIndex = 0;
                await dialog.ShowAsync();
            });

            deferral.Complete();
        }

        public async void OnDescoverSessionAsync(object sender, RoutedEventArgs e)
        {
            if (sessionWatcher != null)
            {
                return;
            }

            // create a watcher for remote system sessions
            sessionWatcher = RemoteSystemSession.CreateWatcher();

            // register a handler for the "added" event
            sessionWatcher.Added += async (s, a) => {

                // get a reference to the info about the discovered session
                RemoteSystemSessionInfo sessionInfo = a.SessionInfo;

                // Optionally update the UI with the sessionInfo.DisplayName and 
                // sessionInfo.ControllerDisplayName strings. 
                // Save a reference to this RemoteSystemSessionInfo to use when the
                // user selects this session from the UI
                //...

                var updateTask = dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    sessionList.Add(sessionInfo);
                });
            };

            // Begin watching
            sessionWatcher.Start();
        }

        public async void OnJoinSessionSelectChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            var session = listView.SelectedItem as RemoteSystemSessionInfo;
            
            if (session!= null)
            {
                var result = await session.JoinAsync();
                                
                Debug.WriteLine($"join {session.DisplayName}: {result.Status.ToString()}");
                
                if (result.Status == RemoteSystemSessionJoinStatus.Success)
                {
                    this.currentSession = result.Session;
                    RegistMessageChannel();
                }
            }
        }

        public void OnSendMessageToSessionClick(object sender, RoutedEventArgs e)
        {

        }

        private void RegistMessageChannel()
        {
            if (appMessageChannel != null)
            {
                appMessageChannel.ValueSetReceived -= AppMessageChannel_ValueSetReceived;
                appMessageChannel = null;
            }

            appMessageChannel = new RemoteSystemSessionMessageChannel(currentSession, "test_channel");
            appMessageChannel.ValueSetReceived += AppMessageChannel_ValueSetReceived;
        }

        private async Task SendMessageToParticipantsAsync(object message)
        {
            using (var stream = new MemoryStream())
            {
                new DataContractJsonSerializer(message.GetType()).WriteObject(stream, message);
                byte[] data = stream.ToArray();
                // Send message to all
                ValueSet sentMessage = new ValueSet { ["Key"] = data };
                // Send specific participants
                await appMessageChannel.BroadcastValueSetAsync(sentMessage);
            }
        }

        private void AppMessageChannel_ValueSetReceived(RemoteSystemSessionMessageChannel sender, RemoteSystemSessionValueSetReceivedEventArgs args)
        {
            ValueSet receivedMessage = args.Message;
        }
        #endregion

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}