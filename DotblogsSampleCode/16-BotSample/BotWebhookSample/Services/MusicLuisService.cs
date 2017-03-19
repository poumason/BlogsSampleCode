using BotWebhookSample.Models.LUIS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace BotWebhookSample.Services
{
    public class MusicLuisService
    {
        private const string API_URL = "";

       /*
        * 1. use LUIS API to convert string to data object
        * 2. parser and build response stastus for receiver
        */

        public string InvokeAPI(string keyword)
        {
            using (WebClient client = new WebClient())
            {
                string encoding = HttpUtility.UrlEncode(keyword);
                string queryString = $"{API_URL}{encoding}";

                client.Encoding = Encoding.UTF8;
                string response = client.DownloadString(queryString);

                LuisResultData jsonResult =  JsonConvert.DeserializeObject<LuisResultData>(response);

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