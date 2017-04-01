using AudioRecordSample.LUISAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AudioRecordSample
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 轉換後的文字
        /// </summary>
        private string translateResult;
        public string TranslateResult
        {
            get { return translateResult; }
            set
            {
                translateResult = value;
                NotifyPropertyChanged("TranslateResult");
            }
        }

        /// <summary>
        /// 目前錄製的時間
        /// </summary>
        private string recordTime;
        public string RecordTime
        {
            get { return recordTime; }
            set
            {
                recordTime = value;
                NotifyPropertyChanged("RecordTime");
            }
        }

        private bool isTranslating;
        public bool IsTranslating
        {
            get { return isTranslating; }
            set
            {
                isTranslating = value;
                NotifyPropertyChanged("IsTranslating");
            }
        }

        private bool? isUseLUISAPI = false;
        public bool? IsUseLUISAPI
        {
            get { return isUseLUISAPI; }
            set
            {
                isUseLUISAPI = value;
                NotifyPropertyChanged("IsUseLUISAPI");
            }
        }

        public List<string> Languages { get; private set; }

        private bool isRecording = false;
        private MediaCapture capture;
        private InMemoryRandomAccessStream buffer;
        private DispatcherTimer timer;
        private DateTime recordStartTime;
        private string translateLanguage = string.Empty;

        public MainPageViewModel()
        {
            Languages = BingSpeechService.SupportLangages;
        }

        public async Task Initialization()
        {
            if (capture != null)
            {
                return;
            }

            // 設定要錄製的 Audio
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Audio
            };

            // 初始化 MediaCapture
            capture = new MediaCapture();
            await capture.InitializeAsync(settings);

            capture.RecordLimitationExceeded += Capture_RecordLimitationExceeded;
            capture.Failed += Capture_Failed;

            buffer = new InMemoryRandomAccessStream();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Capture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            isRecording = false;
        }

        private void Capture_RecordLimitationExceeded(MediaCapture sender)
        {
            isRecording = false;
        }

        int i = 0;

        private void Timer_Tick(object sender, object e)
        {
            if (!isRecording)
            {
                timer.Stop();
                return;
            }

            var diffSpan = DateTime.UtcNow - recordStartTime;
            RecordTime = $"{diffSpan.Minutes.ToString("00")}:{diffSpan.Seconds.ToString("00")}";
        }

        private async Task TranslateAudioToString(IRandomAccessStream audio)
        {
            try
            {
                BingSpeechService service = new BingSpeechService(translateLanguage);
                await service.Initialization();

                var response = await service.SendAudioToAPIAsync(audio);

                var jsonResult = JsonConvert.DeserializeObject<SpeechToTextResultData>(response);

                if (jsonResult.Header.Status == "success")
                {
                    string resultWord = jsonResult.Header.Name.Trim();
                    TranslateResult = resultWord;

                    if (IsUseLUISAPI == true)
                    {
                        MusicLuisService luisService = new MusicLuisService();
                        TranslateResult += "\r\n" + await luisService.InvokeAPI(resultWord);
                    }
                }
                else
                {
                    TranslateResult = response;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task SaveAudioFile(IRandomAccessStream audioStream)
        {
            try
            {
                string fileName = "record.wav";
                StorageFolder folder = ApplicationData.Current.LocalFolder;

                var newFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                using (IRandomAccessStream fileStream = await newFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await RandomAccessStream.CopyAndCloseAsync(audioStream.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                    await audioStream.FlushAsync();
                    audioStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #region UI Event
        public void LanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox control = sender as ComboBox;

            // 記下要使用的語系
            translateLanguage = control.SelectedItem.ToString();
        }

        public async void StartRecord(object sender, RoutedEventArgs e)
        {
            if (isRecording)
            {
                return;
            }

            // 開始錄音
            await capture.StartRecordToStreamAsync(MediaEncodingProfile.CreateWav(AudioEncodingQuality.Auto), buffer);

            isRecording = true;
            recordStartTime = DateTime.UtcNow;
            timer.Start();
        }

        public async void StopRecord(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            isRecording = false;
            IsTranslating = true;

            // 停止錄音
            await capture.StopRecordAsync();

            // 轉成 IRandomAccessStream
            IRandomAccessStream audio = buffer.CloneStream();

            // 轉換成文字
            await TranslateAudioToString(audio);

            IsTranslating = false;
        }
        #endregion

        #region INotifyProperty Event

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}