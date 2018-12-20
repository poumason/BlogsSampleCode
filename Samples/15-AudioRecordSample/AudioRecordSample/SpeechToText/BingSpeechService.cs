using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Networking;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace AudioRecordSample
{
    class BingSpeechService
    {
        /// <summary>
        /// 支援的識別模式
        /// </summary>
        /// <see cref=" https://docs.microsoft.com/en-us/azure/cognitive-services/speech/concepts#recognition-modes"/>
        public static List<string> RecognitionModes = new List<string> { "interactive", "conversation", "dictation" };

        /// <summary>
        /// 預計支援的語系
        /// </summary>
        /// <see cref="https://docs.microsoft.com/en-us/azure/cognitive-services/speech/concepts#recognition-languages"/>
        public static List<string> SupportLangages = new List<string> { "zh-CN", "zh-TW", "zh-HK", "ja-JP", "en-US" };

        private string Language { get; set; }

        private string RecognitionMode { get; set; }

        private string AccessToken { get; set; }

        public BingSpeechService(string lang, string mode)
        {
            if (string.IsNullOrEmpty(lang) || SupportLangages.Contains(lang) == false)
            {
                throw new NotSupportedException("not set default language");
            }

            if (string.IsNullOrEmpty(mode) || RecognitionModes.Contains(mode) == false)
            {
                throw new NotSupportedException("not set recoginition mode");
            }

            Language = lang;
            RecognitionMode = mode;
        }

        public async Task Initialization()
        {
            AccessToken = await Authorization.GetAccessToken();
        }

        private CancellationTokenSource cts;

        public async Task<string> SendAudioToAPIAsync(IRandomAccessStream stream)
        {
            // <OUTPUT_FORMAT>: https://docs.microsoft.com/en-us/azure/cognitive-services/speech/concepts#output-format
            string outputFormat = "detailed";
            string uri = $"https://speech.platform.bing.com/speech/recognition/{RecognitionMode}/cognitiveservices/v1?language={Language}&format={outputFormat}";
            string host = @"speech.platform.bing.com";
            string contentType = @"audio/wav; codec=audio/pcm; samplerate=16000";
            cts = new CancellationTokenSource();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("Bearer", AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("text/xml"));
                client.DefaultRequestHeaders.Host = new HostName(host);
                client.DefaultRequestHeaders.Add("ContentType", contentType);
                client.DefaultRequestHeaders.TransferEncoding.Add(new HttpTransferCodingHeaderValue("chunked"));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(uri));
                request.Content = new HttpStreamContent(stream);

                var response = await client.SendRequestAsync(request).AsTask(cts.Token);

                var buffer = await response.Content.ReadAsBufferAsync();
                var byteArray = buffer.ToArray();
                var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

                return responseString;
            }
        }

        private string GetDeviceIdentifyId()
        {
            string deviceUniqeId = string.Empty;

            try
            {
                if (ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification"))
                {
                    var packageSpecificToken = HardwareIdentification.GetPackageSpecificToken(null);
                    var hardwareId = packageSpecificToken.Id;

                    var hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
                    var hashedHardwareId = hasher.HashData(hardwareId);

                    deviceUniqeId = CryptographicBuffer.EncodeToHexString(hashedHardwareId);
                    return deviceUniqeId;
                }
            }
            catch (Exception)
            {
                // XBOX 目前會取失敗
            }

            // support IoT Device
            var networkProfiles = Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles();
            var adapter = networkProfiles[0].NetworkAdapter;
            string networkAdapterId = adapter.NetworkAdapterId.ToString();
            deviceUniqeId = networkAdapterId.Replace("-", string.Empty);

            return deviceUniqeId;
        }
    }
}