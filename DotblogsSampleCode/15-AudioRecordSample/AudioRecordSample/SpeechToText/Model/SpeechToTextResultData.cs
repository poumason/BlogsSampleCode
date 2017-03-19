using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioRecordSample
{
    public class SpeechToTextResultData
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("header")]
        public HeaderData Header { get; set; }

        [JsonProperty("results")]
        public ResultData[] Results { get; set; }
    }
}