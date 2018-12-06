using KKYoutubeUnoApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KKYoutubeUnoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class YoutubePage : Page
    {
        private YoutubePageViewModel ViewModel;
        private string searchKey;

        public YoutubePage()
        {
            this.InitializeComponent();
            ViewModel = new YoutubePageViewModel();
            this.DataContext = ViewModel;
            Loaded += YoutubePage_Loaded;
            SizeChanged += YoutubePage_SizeChanged;
        }

        private void YoutubePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ViewModel == null)
            {
                return;
            }

            this.ViewModel.WebPlayerWidth = Math.Min(this.ActualWidth, this.ActualHeight / 0.5625);
            this.ViewModel.WebPlayerHeight = Math.Min(this.ActualWidth * 0.58, this.ActualHeight * 0.78);
        }

        private async void YoutubePage_Loaded(object sender, RoutedEventArgs e)
        {
#if !WINDOWS_UWP            
            ViewModel.SafeAreaVisibility = Visibility.Visible;
#endif
            await ViewModel.SearchVideos(searchKey);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var track = e.Parameter as TrackDataWrapper;
            searchKey = $"{track.SongName} {track.ArtistWithAlbumName}";
        }

        public void OnListViewItemClicked(object sender, ItemClickEventArgs e)
        {
            var video = e.ClickedItem as VideoSearchResult;
            ViewModel.ChangeVideo(video.ID, video.Name);
        }

        public void OnBackButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
}