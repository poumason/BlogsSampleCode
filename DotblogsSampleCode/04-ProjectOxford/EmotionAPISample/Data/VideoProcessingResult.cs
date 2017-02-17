using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionAPISample.Data
{
    public class VideoProcessingResult
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("timescale")]
        public int Timescale { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("framerate")]
        public float Framerate { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("fragments")]
        public Fragment[] Fragments { get; set; }
    }

    public class Fragment
    {
        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("interval")]
        public int Interval { get; set; }

        [JsonProperty("events")]
        public Event[][] Events { get; set; }
    }

    public class Event
    {
        [JsonProperty("windowFaceDistribution")]
        public ScoresData WindowFaceDistribution { get; set; }

        [JsonProperty("windowMeanScores")]
        public ScoresData WindowMeanScores { get; set; }
    }
}