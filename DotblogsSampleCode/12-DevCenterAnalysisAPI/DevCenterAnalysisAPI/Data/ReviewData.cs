using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCenterAnalysisAPI
{
    public class AnalysisData<T>
        where T: class
    {
        [JsonProperty("Value")]
        public T[] Value { get; set; }

        [JsonProperty("nextLink")]
        public string nextLink { get; set; }

        [JsonProperty("TotalCount")]
        public int TotalCount { get; set; }
    }

    public class ReviewData
    {
        [JsonProperty("date")]
        public string Date { get; set; }
        public string applicationId { get; set; }
        public string applicationName { get; set; }
        public string market { get; set; }
        public string osVersion { get; set; }
        public string deviceType { get; set; }
        public bool isRevised { get; set; }
        public string packageVersion { get; set; }
        public string deviceModel { get; set; }
        public string productFamily { get; set; }
        public int deviceRAM { get; set; }
        public string deviceScreenResolution { get; set; }
        public int deviceStorageCapacity { get; set; }
        public bool isTouchEnabled { get; set; }
        public string reviewerName { get; set; }
        public int rating { get; set; }
        public string reviewTitle { get; set; }

        [JsonProperty("reviewText")]
        public string ReviewText { get; set; }
        public int helpfulCount { get; set; }
        public int notHelpfulCount { get; set; }
        public object responseDate { get; set; }
        public string responseText { get; set; }
    }

}