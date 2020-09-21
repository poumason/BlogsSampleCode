using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace mp3WavConverter
{
    public class STT
    {
        [JsonIgnore]
        public string Display { get; set; }

        [JsonIgnore]
        public double AudioLengthInSeconds { get; set; }

        //[JsonIgnore]
        public Segmentresult[] SegmentResults { get; set; }

        [JsonIgnore]
        public List<WordData> Words { get; set; }

        public STT()
        {
            Words = new List<WordData>();
        }
    }
}