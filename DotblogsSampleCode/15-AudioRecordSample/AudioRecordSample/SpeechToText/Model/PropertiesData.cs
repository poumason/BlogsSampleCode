using Newtonsoft.Json;

namespace AudioRecordSample
{
    public class PropertiesData
    {
        /// <summary>
        /// this is a uuid identifying the requestid to be sent with the associated logs. It should be used as the "server.requestid" parameter value in the subsequent logging API request. 
        /// </summary>
        [JsonProperty("requestid")]
        public string Requestid { get; set; }

        /// <summary>
        /// 1: set when no speech was detected on the sent audio. 
        /// </summary>
        [JsonProperty("NOSPEECH")]
        public string NOSPEECH { get; set; } = null;

        /// <summary>
        /// 1: set when no matches were found for the sent audio. 
        /// </summary>
        [JsonProperty("FALSERECO")]
        public string FALSERECO { get; set; } = null;

        /// <summary>
        /// 1: set when the header result is determined to be of high-confidence. 
        /// </summary>
        [JsonProperty("HIGHCONF")]
        public string HIGHCONF { get; set; } = null;

        /// <summary>
        /// 1: set when the header result is determined to be of medium-confidence.
        /// </summary>
        [JsonProperty("MIDCONF")]
        public string MIDCONF { get; set; } = null;

        /// <summary>
        /// 1: set when the header result is determined to be of low-confidence.
        /// </summary>
        [JsonProperty("LOWCONF")]
        public string LOWCONF { get; set; } = null;

        /// <summary>
        /// 1: set when there was an error generating a response. 
        /// </summary>
        [JsonProperty("ERROR")]
        public string ERROR { get; set; } = null;
    }
}