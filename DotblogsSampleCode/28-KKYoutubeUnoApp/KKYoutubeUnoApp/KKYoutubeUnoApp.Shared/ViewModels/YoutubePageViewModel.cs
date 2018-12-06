using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KKYoutubeUnoApp.ViewModels
{
    /// <summary>
    /// <see cref="https://github.com/youtube/api-samples/blob/master/dotnet/Google.Apis.YouTube.Samples.Search/Search.cs"/>
    /// </summary>
    public class YoutubePageViewModel : BindableBase
    {
        private readonly YouTubeService youtubeService;

        private bool isSearching = false;
        public bool IsSearching
        {
            get { return isSearching; }
            set
            {
                isSearching = value;
                OnPropertyChanged(nameof(IsSearching));
            }
        }

        private Uri playingVideoUrl;
        public Uri PlayingVideoUrl
        {
            get { return playingVideoUrl; }
            set
            {
                playingVideoUrl = value;
                OnPropertyChanged(nameof(PlayingVideoUrl));
            }
        }

        private double webPlayerWidth;
        public double WebPlayerWidth
        {
            get { return webPlayerWidth; }
            set
            {
                webPlayerWidth = value;
                OnPropertyChanged(nameof(WebPlayerWidth));
            }
        }

        private double webPlayerHeight;
        public double WebPlayerHeight
        {
            get { return webPlayerHeight; }
            set
            {
                webPlayerHeight = value;
                OnPropertyChanged(nameof(WebPlayerHeight));
            }
        }

        private Visibility safeAreaVisibility = Visibility.Collapsed;
        public Visibility SafeAreaVisibility
        {
            get { return safeAreaVisibility; }
            set
            {
                safeAreaVisibility = value;
                OnPropertyChanged(nameof(SafeAreaVisibility));
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public ObservableCollection<VideoSearchResult> Videos { get; private set; }

        public YoutubePageViewModel()
        {
            youtubeService = new YouTubeService(new Google.Apis.Services.BaseClientService.Initializer
            {
                ApiKey = "",
                ApplicationName = this.GetType().ToString()
            });

            Videos = new ObservableCollection<VideoSearchResult>();
        }

        public async Task SearchVideos(string keyword)
        {
            if (IsSearching)
            {
                return;
            }

            IsSearching = true;

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = keyword;
            searchListRequest.MaxResults = 50;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            var videoResults = searchListResponse.Items.Where(x => x.Id.Kind == "youtube#video");

            Videos.Clear();

            foreach (var item in videoResults)
            {
                Videos.Add(new VideoSearchResult(item));
            }

            var first = Videos.FirstOrDefault();

            if (first != null)
            {
                ChangeVideo(first.ID, first.Name);
            }

            OnPropertyChanged(nameof(Videos));

            IsSearching = false;
        }

        public void ChangeVideo(string id, string title)
        {
            // 轉換成真的能播放的網站
            string url = $"https://myyoutube.azure.com/embedYoutube.html?video={id}";
            PlayingVideoUrl = new Uri(url);
            Title = title;
        }
    }

    public class VideoSearchResult
    {
        private SearchResult source;

        public string VideoImageUrl
        {
            get { return source?.Snippet?.Thumbnails?.Default__?.Url; }
        }

        public string Name
        {
            get { return source.Snippet.Title; }
        }

        public string ID
        {
            get { return source.Id.VideoId; }
        }

        public VideoSearchResult(SearchResult data)
        {
            source = data;
        }
    }
}