using Newtonsoft.Json;

namespace AudioRecordSample.LUISAPI
{
    public class DialogData
    {
        [JsonProperty("contextId")]
        public string contextId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}