using EmotionAPISample.Data;
using EmotionAPISample.Library;
using EmotionAPISample.Utility;
using EmotionAPISample.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EmotionAPISample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoPage : Page
    {
        private MenuFlyout menuFlyout;
        private IRandomAccessStream fileStream;

        public VideoPage()
        {
            this.InitializeComponent();
            PrepareFlyoutItesm();
        }

        private void OnCapturePhoto(object sender, RoutedEventArgs e)
        {
            string content = "";
            var progressResult = JsonConvert.DeserializeObject<VideoProcessingResult>(content);

            //Button btn = sender as Button;
            //menuFlyout.ShowAt(btn);
        }

        #region FlyoutItem 
        private void PrepareFlyoutItesm()
        {
            menuFlyout = new MenuFlyout();
            MenuFlyoutItem camItem = new MenuFlyoutItem();
            MenuFlyoutItem phototem = new MenuFlyoutItem();
            phototem.Text = "Video Library";
            phototem.Tag = "video";
            phototem.Click += MenuItem_Click;
            menuFlyout.Items.Add(phototem);
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file;
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mov");

            file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                fileStream = await file.OpenAsync(FileAccessMode.Read);
                videoPlayer.SetSource(fileStream, "mpeg/video");
            }
        }
        #endregion

        private async void OnAnalysisByEmotionAPI(object sender, RoutedEventArgs e)
        {
            VideoOperationResult videoResult = null;
            byte[] data = UtilityHelper.StreamToByteAraray(fileStream.AsStreamForRead());
            string key = "";
            EmotionAPIService api = new EmotionAPIService(key);
            api.VideoAnalysisProgressChanged += Api_VideoAnalysisProgressChanged;
            api.VideoAnalysisSuccessed += Api_VideoAnalysisSuccessed;
            videoResult = await api.RecognizeVideo(data.AsBuffer(), string.Empty);
        }

        private void Api_VideoAnalysisSuccessed(object sender, VideoProcessingResult e)
        {
            if (e != null)
            {
            }
        }

        private void Api_VideoAnalysisProgressChanged(object sender, double e)
        {
            Debug.WriteLine($"video progress ... : {e}");
        }
    
    }
}