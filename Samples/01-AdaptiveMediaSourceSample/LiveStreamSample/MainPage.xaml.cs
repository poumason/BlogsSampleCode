using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LiveStreamSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task<Uri> GetHitChannelHSL(String url, String tag)
        {
            Uri resultUri = null;
            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(url);
                request.CookieContainer = new CookieContainer();
                request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
                request.AllowReadStreamBuffering = true;

                var resposne = await request.GetResponseAsync();

                using (StreamReader stream = new StreamReader(resposne.GetResponseStream()))
                {
                    while (stream.Peek() >= 0)
                    {
                        String strLine = stream.ReadLine();
                        Debug.WriteLine(strLine);
                        if (strLine.Contains(tag))
                        {
                            String[] para = strLine.Split('\'');
                            String urlStr = para[1].Replace("\\", "");
                            resultUri = new Uri(urlStr);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }
            return resultUri;
        }

        private AdaptiveMediaSource _source;

        private async void OnPlayClick(object sender, RoutedEventArgs e)
        {
            String url = txtURL.Text;
            String tag = "ra000001";
            Uri hsl = await GetHitChannelHSL(url, tag);
            if (hsl != null)
            {
                // check is phone
                bool isHardwareButtonsAPIPresent =
        Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");

                if (isHardwareButtonsAPIPresent)
                {
                    //Windows.Phone.UI.Input.HardwareButtons.CameraPressed +=
                    //    HardwareButtons_CameraPressed;
                    ValueSet msg = new ValueSet();
                    msg.Add("Play", hsl.OriginalString);
                    BackgroundMediaPlayer.SendMessageToBackground(msg);
                }

                AdaptiveMediaSourceCreationResult result = await AdaptiveMediaSource.CreateFromUriAsync(hsl);
                if (result.Status == AdaptiveMediaSourceCreationStatus.Success)
                {
                    _source = result.MediaSource;
                    _source.DownloadRequested += _source_DownloadRequested;
                    _source.DownloadCompleted += _source_DownloadCompleted;
                    _source.DownloadFailed += _source_DownloadFailed;
                    _source.DownloadBitrateChanged += _source_DownloadBitrateChanged;
                    _source.PlaybackBitrateChanged += _source_PlaybackBitrateChanged;

                    mediaPlayer.SetMediaStreamSource(result.MediaSource);
                }
            }
        }

        private void _source_PlaybackBitrateChanged(AdaptiveMediaSource sender, AdaptiveMediaSourcePlaybackBitrateChangedEventArgs args)
        {
         
        }

        private void _source_DownloadBitrateChanged(AdaptiveMediaSource sender, AdaptiveMediaSourceDownloadBitrateChangedEventArgs args)
        {

        }

        private void _source_DownloadFailed(AdaptiveMediaSource sender, AdaptiveMediaSourceDownloadFailedEventArgs args)
        {
            
        }

        private void _source_DownloadCompleted(AdaptiveMediaSource sender, AdaptiveMediaSourceDownloadCompletedEventArgs args)
        {
        }

        private void _source_DownloadRequested(AdaptiveMediaSource sender, AdaptiveMediaSourceDownloadRequestedEventArgs args)
        {
        }

        private void OnPauseClick(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }
    }
}