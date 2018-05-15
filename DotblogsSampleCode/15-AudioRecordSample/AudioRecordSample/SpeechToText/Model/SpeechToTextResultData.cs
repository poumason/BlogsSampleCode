using Newtonsoft.Json;

namespace AudioRecordSample
{
    public class SpeechToTextResultData
    {
        [JsonProperty("RecognitionStatus")]
        public string Status { get; set; }

        [JsonProperty("Offset")]
        public long Offset { get; set; }

        [JsonProperty("Duration")]
        public long Duration { get; set; }

        [JsonProperty("NBest")]
        public ResultData[] Results { get; set; }

        [JsonProperty("DisplayText")]
        public string DisplayText { get; set; }
    }
}