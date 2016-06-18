using EmotionAPISample.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionAPISample.Converter
{
    public class StringEnumConverter : JsonConverter
    {
        const string NOT_STARTED = "Not Started";

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;
            if (enumString == NOT_STARTED)
            {
                return VideoOperationStatus.NotStarted;
            }
            else
            {
                return Enum.Parse(typeof(VideoOperationStatus), enumString, true);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            VideoOperationStatus status = (VideoOperationStatus)value;
            if (status == VideoOperationStatus.NotStarted)
            {
                writer.WriteValue(NOT_STARTED);
            }else
            {
                writer.WriteValue(status.ToString());
            }
        }
    }
}