using Microsoft.AdMediator.Core.Models;
using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            // add this during initialization of your app

            AdMediator_826892.AdSdkError += AdMediator_826892_AdError;
            AdMediator_826892.AdMediatorFilled += AdMediator_826892_AdFilled;
            AdMediator_826892.AdMediatorError += AdMediator_826892_AdMediatorError;
            AdMediator_826892.AdSdkEvent += AdMediator_826892_AdSdkEvent;
        }

        // and then add these functions

        void AdMediator_826892_AdSdkEvent(object sender, Microsoft.AdMediator.Core.Events.AdSdkEventArgs e)
        {
            Debug.WriteLine("AdSdk event {0} by {1}", e.EventName, e.Name);
        }

        void AdMediator_826892_AdMediatorError(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
        {
            Debug.WriteLine("AdMediatorError:" + e.Error + " " + e.ErrorCode);
            // if (e.ErrorCode == AdMediatorErrorCode.NoAdAvailable)
            // AdMediator will not show an ad for this mediation cycle
        }

        void AdMediator_826892_AdFilled(object sender, Microsoft.AdMediator.Core.Events.AdSdkEventArgs e)
        {
            Debug.WriteLine("AdFilled:" + e.Name);
        }

        void AdMediator_826892_AdError(object sender, Microsoft.AdMediator.Core.Events.AdFailedEventArgs e)
        {
            Debug.WriteLine("AdSdkError by {0} ErrorCode: {1} ErrorDescription: {2} Error: {3}", e.Name, e.ErrorCode, e.ErrorDescription, e.Error);
        }


        private void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
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

        private void ModifyParameter()
        {
            AdMediator_826892.AdSdkOptionalParameters[AdSdkNames.Smaato]["Width"] = 400;
            AdMediator_826892.AdSdkOptionalParameters[AdSdkNames.Smaato]["Height"] = 80;
        }
    }
}
