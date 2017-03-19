using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.Web.Http;

namespace AudioRecordSample
{
    class BingSpeechService
    {
        const string RecognizeUri = "https://speech.platform.bing.com/recognize";

        private string AccessToken { get; set; }
        private string QueryString { get; set; }

        /// <summary>
        /// 預計支援的語系，更多可參考： https://www.microsoft.com/cognitive-services/en-us/speech-api/documentation/overview
        /// </summary>
        public static List<string> SupportLangages = new List<string> { "zh-CN", "zh-TW", "zh-HK", "ja-JP", "en-US" };

        public BingSpeechService(string lang)
        {
            if (string.IsNullOrEmpty(lang) || SupportLangages.Contains(lang) == false)
            {
                throw new NotSupportedException("not support language");
            }

            // Always use appID = D4D52672-91D7-4C74-8AD8-42B1D98141A5.  (GUID)
            string appId = "D4D52672-91D7-4C74-8AD8-42B1D98141A5";

            // https://www.microsoft.com/cognitive-services/en-us/Speech-api/documentation/overview
            string locale = lang;

            // Windows OS, Windows Phone OS, XBOX, Android, iPhone OS
            string deviceOS = "Windows10";

            // A globally unique device identifier of the device making the request (GUID)
            string instanceid = "565D69FF-E928-4B7E-87DA-9A750B96D9E3";

            QueryString = $"scenarios=smd&appid={appId}&locale={locale}&device.os={deviceOS}&version=3.0&format=json&instanceid={instanceid}";
        }

        public async Task Initialization()
        {
            AccessToken = await Authorization.GetAccessToken();
        }

        public async Task<string> SendAudioToAPIAsync(IRandomAccessStream stream)
        {
            string host = @"speech.platform.bing.com";
            string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";

            using (HttpClient client = new HttpClient())
            {
                // request id is (GUID)
                string uri = $"{RecognizeUri}?{QueryString}&requestid={Guid.NewGuid().ToString()}";

                client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Bearer", AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("text/xml"));
                client.DefaultRequestHeaders.Host = new Windows.Networking.HostName(host);
                client.DefaultRequestHeaders.Add("ContentType", contentType);

                HttpStreamContent streamContent = new HttpStreamContent(stream);

                var response = await client.PostAsync(new Uri(uri), streamContent);
                
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