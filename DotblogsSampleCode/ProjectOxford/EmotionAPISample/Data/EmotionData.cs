using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionAPISample.Data
{
    public class EmotionData
    {
        [JsonProperty("faceRectangle")]
        public FacerectangleData FaceRectangle { get; set; }

        [JsonProperty("scores")]
        public ScoresData Scores { get; set; }
    }
}
