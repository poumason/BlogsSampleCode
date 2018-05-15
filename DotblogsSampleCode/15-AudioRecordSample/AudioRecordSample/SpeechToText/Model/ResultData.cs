using Newtonsoft.Json;

namespace AudioRecordSample
{
    public class ResultData
    {

        /// <summary>
        /// text of what was spoken. Profane terms are surrounded with <profanity> tags.
        /// </summary>
        [JsonProperty("Lexical")]
        public string Lexical { get; set; }

        [JsonProperty("Confidence")]
        public string Confidence { get; set; }

        [JsonProperty("ITN")]
        public string ITN { get; set; }

        [JsonProperty("MaskedITN")]
        public string MaskedITN { get; set; }

        [JsonProperty("Display")]
        public string Display { get; set; }
    }
}