using SingleBackgroundMediaPlayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace WPFAndUWPSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlayerService player;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnSendToastClick(object sender, RoutedEventArgs e)
        {
            SendToast();
            UpdateTile();
        }

        private void OnRegistSMTCClick(object sender, RoutedEventArgs e)
        {
            if (player != null)
            {
                return;
            }

            player = new PlayerService();

            MediaPlaybackList = new MediaPlaybackList();
            MediaPlaybackList.AutoRepeatEnabled = true;
            MediaPlaybackList.CurrentItemChanged += MediaPlaybackList_CurrentItemChanged;
            BuildMediaPlaybackList();

            player.PlaybackList = MediaPlaybackList;
            player.Play();
        }

        private void MediaPlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            Debug.WriteLine(args);
        }

        #region STMC
        public MediaPlaybackList MediaPlaybackList { get; private set; }

        private void BuildMediaPlaybackList()
        {
            // 建立 MediaPlaybackList，並設定 MusicProperties，讓 SMTC 更新 UI
            for (int i = 1; i < 5; i++)
            {
                MediaSource source = MediaSource.CreateFromUri(new Uri($"{Package.Current.InstalledLocation.Path}/WPFAndUWPSample/Assets/mp3/0{i}.mp3", UriKind.RelativeOrAbsolute));
                MediaPlaybackItem item = new MediaPlaybackItem(source);
                MediaItemDisplayProperties displayProperty = item.GetDisplayProperties();
                displayProperty.Type = MediaPlaybackType.Music;
                displayProperty.MusicProperties.Title = $"0{i}.mp3";
                displayProperty.MusicProperties.AlbumArtist = "JJ";
                displayProperty.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri($"{Package.Current.InstalledLocation.Path}/WPFAndUWPSample/Assets/mp3/0{i}.jpg")); ;
                item.ApplyDisplayProperties(displayProperty);

                MediaPlaybackList.Items.Add(item);
            }
        }

        private void UseBackgroundMediaPlayer()
        {
            BackgroundMediaPlayer.Current.SystemMediaTransportControls.ButtonPressed += Smtc_ButtonPressed;

            var updater  = BackgroundMediaPlayer.Current.SystemMediaTransportControls.DisplayUpdater;
            updater.MusicProperties.Title = "song name";
            updater.MusicProperties.AlbumArtist = "artsit and album";
            updater.Update();
        }

        private void Smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            // 處理在 SMTC 操作的事件
        }

        #endregion


        private async void GetCurrentPosition()
        {
            var locator = new Windows.Devices.Geolocation.Geolocator();
            var location = await locator.GetGeopositionAsync();
            var position = location.Coordinate.Point.Position;
            var latlong = string.Format("lat:{0}, long:{1}", position.Latitude, position.Longitude);
            var result = MessageBox.Show(latlong);
        }

        private void SendToast()
        {
            string title = "featured picture of the day";
            string content = "beautiful scenery";
            string image = "https://picsum.photos/360/180?image=104";
            string logo = "https://picsum.photos/64?image=883";

            string xmlString =
            $@"<toast><visual>
       <binding template='ToastGeneric'>
       <text>{title}</text>
       <text>{content}</text>
       <image src='{image}'/>
       <image src='{logo}' placement='appLogoOverride' hint-crop='circle'/>
       </binding>
      </visual></toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlString);

            ToastNotification toast = new ToastNotification(toastXml);

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private async Task UpdateTile()
        {
            //// Create a tile update manager for the specified syndication feed.
            //var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            //updater.EnableNotificationQueue(true);
            //updater.Clear();

            //// Keep track of the number feed items that get tile notifications.
            //int itemCount = 0;

            //// Create a tile notification for each feed item.
            //foreach (var item in feed.Items)
            //{
            //    XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText03);

            //    var title = item.Title;
            //    string titleText = title.Text == null ? String.Empty : title.Text;
            //    tileXml.GetElementsByTagName(textElementName)[0].InnerText = titleText;

            //    // Create a new tile notification.
            //    updater.Update(new TileNotification(tileXml));

            //    // Don't create more than 5 notifications.
            //    if (itemCount++ > 5) break;
            //}

            // Initialize the tile with required arguments
            SecondaryTile tile = new SecondaryTile(
                "myTileId5391",
                "Display name",
                "myActivationArgs",
                new Uri("ms-appx:///Assets/Square150x150Logo.png"),
                TileSize.Default);

            // Assign the window handle
            IInitializeWithWindow initWindow = (IInitializeWithWindow)(object)tile;
            initWindow.Initialize(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);

            // Pin the tile
            bool isPinned = await tile.RequestCreateAsync();

            // TODO: Update UI to reflect whether user can now either unpin or pin
        }

        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize(IntPtr hwnd);
        }
    }
}