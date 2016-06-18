using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionAPISample.Data
{
    public class FacerectangleData
    {
        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("left")]
        public int Left { get; set; }

        [JsonProperty("top")]
        public int Top { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        public override string ToString()
        {
            return $"{Left},{Top},{Width},{Height}";
        }
    }
}