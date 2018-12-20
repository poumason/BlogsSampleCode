using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AudioRecordSample.LUISAPI
{
    public class MusicLuisService
    {
        private const string API_URL = "";

        /*
         * 1. use LUIS API to convert string to data object
         * 2. parser and build response stastus for receiver
         */

        public async Task<string> InvokeAPI(string keyword)
        {
            using (HttpClient client = new HttpClient())
            {
                string encoding = WebUtility.UrlEncode(keyword);
                string queryString = $"{API_URL}{encoding}";

                client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("utf-8"));
                string response = await client.GetStringAsync(queryString);

                LuisResultData jsonResult = JsonConvert.DeserializeObject<LuisResultData>(response);

                return BuildQueryResult(keyword, jsonResult);
            }
        }

        private string BuildQueryResult(string query, LuisResultData lineData)
        {
            var topIntent = lineData.TopScoringIntent;
            var entities = lineData.Entities;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"query: {query}");
            builder.AppendLine($"intent: {topIntent.Intent}, score: {topIntent.Score}");
            foreach (var item in entities)
            {
                builder.AppendLine($"entity: {item.entity}, index: {item.StartIndex},{item.EndIndex}, type: {item.Type}, score: {item.Score}");
            }

            return builder.ToString();
        }
    }
}
