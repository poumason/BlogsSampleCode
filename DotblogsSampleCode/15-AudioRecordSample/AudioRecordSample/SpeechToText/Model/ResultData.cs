using Newtonsoft.Json;

namespace AudioRecordSample
{
    public class ResultData
    {
        /// <summary>
        /// the scenario this recognition result came from.
        /// </summary>
        [JsonProperty("scenario")]
        public string Scenario { get; set; }

        /// <summary>
        /// formatted recognition result. Profane terms are surrounded with <profanity> tags. 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// text of what was spoken. Profane terms are surrounded with <profanity> tags.
        /// </summary>
        [JsonProperty("lexical")]
        public string Lexical { get; set; }

        [JsonProperty("confidence")]
        public string Confidence { get; set; }

        /// <summary>
        /// HIGHCONF, MIDCONF, LOWCONF
        /// </summary>
        [JsonProperty("properties")]
        public PropertiesData Properties { get; set; }
    }
}