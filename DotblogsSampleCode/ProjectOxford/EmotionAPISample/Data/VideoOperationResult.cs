using EmotionAPISample.Converter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionAPISample.Data
{
    public enum VideoOperationStatus
    {
        NotStarted,
        Uploading,
        Running,
        Failed,
        Succeeded,
    }

    public class VideoOperationResult
    {
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public VideoOperationStatus Status { get; set; }

        [JsonProperty("progress")]
        public float Progress { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("lastActionDateTime")]
        public DateTime LastActionDateTime { get; set; }

        [JsonProperty("processingResult")]
        public string ProcessingResult { get; set; }
    }
}