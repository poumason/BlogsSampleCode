using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;

namespace BackgroundAudioTask
{
    public sealed class BackgroundAudioTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private SystemMediaTransportControls systemControl;
        private AutoResetEvent BackgroundTaskStarted = new AutoResetEvent(false);
        private AdaptiveMediaSource _source;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            systemControl = SystemMediaTransportControls.GetForCurrentView();

            systemControl.ButtonPressed -= SystemControls_ButtonPressed;
            systemControl.ButtonPressed += SystemControls_ButtonPressed;

            taskInstance.Canceled -= TaskInstance_Canceled;
            taskInstance.Canceled += TaskInstance_Canceled;
            taskInstance.Task.Completed -= Task_Completed;
            taskInstance.Task.Completed += Task_Completed;

            BackgroundMediaPlayer.MessageReceivedFromForeground -= BackgroundMediaPlayer_MessageReceivedFromForeground;
            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;

            BackgroundTaskStarted.Set();
        }

        void Task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            if (deferral != null)
            {
                deferral.Complete();
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            BackgroundMediaPlayer.Shutdown();
            if (deferral != null)
            {
                deferral.Complete();
            }
        }

        void SystemControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
        }

        async void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                switch (key.ToLower())
                {
                    case "play":
                        String url =e.Data[key].ToString();
                        await PlayHLS(new Uri(url, UriKind.RelativeOrAbsolute));
                        break;
                    case "stop":
                        BackgroundMediaPlayer.Current.Pause();
                        break;
                    case "wakup_background_player":
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task PlayHLS(Uri hsl)
        {
            AdaptiveMediaSourceCreationResult result = await AdaptiveMediaSource.CreateFromUriAsync(hsl);
            if (result.Status == AdaptiveMediaSourceCreationStatus.Success)
            {
                _source = result.MediaSource;
                _source.DownloadRequested += _source_DownloadRequested;
                _source.DownloadCompleted += _source_DownloadCompleted;
                _source.DownloadFailed += _source_DownloadFailed;
                _source.DownloadBitrateChanged += _source_DownloadBitrateChanged;
                _source.PlaybackBitrateChanged += _source_PlaybackBitrateChanged;

                BackgroundMediaPlayer.Current.SetMediaSource(result.MediaSource);
            }
        }

        #region AdaptiveMediaSource Events
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
        #endregion
    }
}