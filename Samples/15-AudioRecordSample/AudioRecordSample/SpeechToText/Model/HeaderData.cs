using Newtonsoft.Json;

namespace AudioRecordSample
{
    public class HeaderData
    {
        /// <summary>
        /// "success"/ "error"/ "false reco"// 'false reco' is returned only for 2.0 responses when NOSPEECH OR FALSERECO is 1. This is done to maintain backward compatibility. 
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

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

        [JsonProperty("properties")]
        public PropertiesData Properties { get; set; }
    }
}