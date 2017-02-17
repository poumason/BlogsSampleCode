using Microsoft.Advertising.WinRT.UI;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaAdvertisingSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loading += MainPage_Loading;
        }

        private void MainPage_Loading(FrameworkElement sender, object args)
        {
            AddInterstitialAdControl();
        }

        #region InterstitialAd

        private InterstitialAd intersitialAdControl;

        private void AddInterstitialAdControl()
        {
            string adUnitId = "11389925";
            string applicationId = "d25517cb-12d4-4699-8bdc-52040c712cab";

            intersitialAdControl = new InterstitialAd();
            intersitialAdControl.AdReady += IntersitialAdControl_AdReady;
            intersitialAdControl.Cancelled += IntersitialAdControl_Cancelled;
            intersitialAdControl.Completed += IntersitialAdControl_Completed;
            intersitialAdControl.ErrorOccurred += IntersitialAdControl_ErrorOccurred;

            intersitialAdControl.RequestAd(AdType.Video, applicationId, adUnitId);
        }

        private void IntersitialAdControl_ErrorOccurred(object sender, AdErrorEventArgs e)
        {
            // 需處理發生錯誤的狀況
            if (e.ErrorCode == Microsoft.Advertising.ErrorCode.NoAdAvailable)
            {
                // 這個是最常遇到的，這個時候就不要顯示廣告或是更換 InterstitialAd 的參數在做請求
            }
        }

        private void IntersitialAdControl_Completed(object sender, object e)
        {
            // 當廣告影片播放完畢時被觸發
        }

        private void IntersitialAdControl_Cancelled(object sender, object e)
        {
            // 當廣告播放到一半被取消時
            intersitialAdControl = null;
        }

        private void IntersitialAdControl_AdReady(object sender, object e)
        {
            // 當 ready 的時候再顯示廣告内容
            if ((InterstitialAdState.Ready) == (intersitialAdControl.State))
            {
                intersitialAdControl.Show();
            }
        }
        #endregion

        private void AdMediator_826892_ErrorOccurred(object sender, AdErrorEventArgs e)
        {
            Debug.WriteLine("AdSdkError by ErrorCode: {0} ErrorDescription: {1}", e.ErrorCode, e.ErrorMessage);
        }

        private void AdMediator_826892_AdRefreshed(object sender, RoutedEventArgs e)
        {

        }
    }
}
