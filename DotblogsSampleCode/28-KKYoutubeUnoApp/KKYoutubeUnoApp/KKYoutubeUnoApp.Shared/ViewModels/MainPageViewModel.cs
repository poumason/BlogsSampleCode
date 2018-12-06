using KKBOX.OpenAPI;
using KKBOX.OpenAPI.ServiceModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KKYoutubeUnoApp.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        const string clientId = "";
        const string clientSecret = "";

        private string searchKeyword;
        public string SearchKeyWord
        {
            get { return searchKeyword; }
            set
            {
                searchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyWord));
            }
        }

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

        public ObservableCollection<TrackDataWrapper> Tracks { get; private set; }

        private KKBOXAPI apiClient;

        public MainPageViewModel()
        {
            apiClient = new KKBOXAPI();
            Tracks = new ObservableCollection<TrackDataWrapper>();
        }

        public async Task InitAPI()
        {
            var authResult = await KKBOXOAuth.SignInAsync(clientId, clientSecret);
            apiClient.AccessToken = authResult.Content.AccessToken;
        }

        public async Task SearchAsync()
        {
            if (string.IsNullOrEmpty(searchKeyword))
            {
                return;
            }

            if (IsSearching)
            {
                return;
            }

            IsSearching = true;
            var searchResult = await apiClient.SearchAsync(SearchKeyWord, 30, 0, SearchType.track);

            Tracks.Clear();

            foreach (var item in searchResult.Content.Tracks.Data)
            {
                Tracks.Add(new TrackDataWrapper(item));
            }

            OnPropertyChanged(nameof(Tracks));

            IsSearching = false;
        }
    }
}