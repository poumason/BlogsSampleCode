using EmotionAPISample.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace EmotionAPISample.Library
{
    public class EmotionAPIService : IDisposable
    {
        public event EventHandler<double> VideoAnalysisProgressChanged;

        public event EventHandler<VideoProcessingResult> VideoAnalysisSuccessed;

        private Dictionary<RecognitionType, string> RecognitionUrl { get; set; }

        private HttpClient httpClient { get; set; }

        public EmotionAPIService(string apiKey)
        {
            RecognitionUrl = new Dictionary<RecognitionType, string>();
            RecognitionUrl.Add(RecognitionType.Image, "https://api.projectoxford.ai/emotion/v1.0/recognize");
            RecognitionUrl.Add(RecognitionType.Video, "https://api.projectoxford.ai/emotion/v1.0/recognizeinvideo");
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
        }

        public async Task<List<EmotionData>> Recognize(string url, string queryString)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            else
            {
                var jsonData = new
                {
                    url = url
                };
                string json = JsonConvert.SerializeObject(jsonData);
                byte[] data = Encoding.UTF8.GetBytes(json);
                return await InvokeEmotionAPIForImage(data.AsBuffer(), "application/json", queryString);
            }
        }

        public async Task<List<EmotionData>> Recognize(IBuffer buffer, string queryString)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return null;
            }
            else
            {
                return await InvokeEmotionAPIForImage(buffer, "application/octet-stream", queryString);
            }
        }

        public async Task<VideoOperationResult> RecognizeVideo(IBuffer buffer, string queryString)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return null;
            }
            else
            {
                return await InvokeEmotionAPIForVideo(buffer, queryString);
            }
        }

        private async Task<List<EmotionData>> InvokeEmotionAPIForImage(IBuffer buffer, string contentType, string queryString)
        {
            Uri uri = new Uri($"{RecognitionUrl[RecognitionType.Image]}?{queryString}");
            HttpBufferContent content = new HttpBufferContent(buffer);
            content.Headers.ContentType = new HttpMediaTypeHeaderValue(contentType);

            var response = await httpClient.PostAsync(uri, content);
            if (response.StatusCode == HttpStatusCode.Ok)
            {
                string jsonContent = await response.Content.ReadAsStringAsync();
                var emotionResponse = JsonConvert.DeserializeObject<List<EmotionData>>(jsonContent);
                return emotionResponse;
            }
            else
            {
                return null;
            }
        }

        private async Task<VideoOperationResult> InvokeEmotionAPIForVideo(IBuffer buffer, string queryString)
        {
            VideoOperationResult result = null;
            Uri uri = new Uri($"{RecognitionUrl[RecognitionType.Video]}?{queryString}");
            HttpBufferContent content = new HttpBufferContent(buffer);
            content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/octet-stream");

            var response = await httpClient.PostAsync(uri, content);
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                string location = string.Empty;
                response.Headers.TryGetValue("Operation-Location", out location);
                if (string.IsNullOrEmpty(location) == false)
                {
                    Uri operationUri = new Uri(location);
                    var locationResponse = await httpClient.GetAsync(operationUri);
                    string jsonResult = await locationResponse.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<VideoOperationResult>(jsonResult);
                    ProcessVideoResult(result);
                    if (result.Status == VideoOperationStatus.Running)
                    {
                        var task = MonitorVideoProgress(operationUri);
                    }
                }
            }
            return result;
        }

        private async Task MonitorVideoProgress(Uri location)
        {
            int delaySecond = 20;

            while (true)
            {
                var response = await httpClient.GetStringAsync(location);
                var videoResult = JsonConvert.DeserializeObject<VideoOperationResult>(response);
                ProcessVideoResult(videoResult);
                if (videoResult.Status == VideoOperationStatus.Succeeded)
                {
                    break;
                }
                await Task.Delay(TimeSpan.FromSeconds(delaySecond));
            }

            await Task.CompletedTask;
        }

        private void ProcessVideoResult(VideoOperationResult result)
        {
            switch (result.Status)
            {
                case VideoOperationStatus.Succeeded:
                    var progressResult = JsonConvert.DeserializeObject<VideoProcessingResult>(result.ProcessingResult);                        
                    VideoAnalysisSuccessed?.Invoke(this, progressResult);
                    break;
                case VideoOperationStatus.Running:
                    VideoAnalysisProgressChanged(this, result.Progress);
                    break;
                case VideoOperationStatus.Uploading:
                case VideoOperationStatus.Failed:
                case VideoOperationStatus.NotStarted:
                    break;
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}