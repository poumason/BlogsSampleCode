using Newtonsoft.Json;

namespace OAuthBotAppSample.Google
{
    public class UserProfile
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }
    }
}