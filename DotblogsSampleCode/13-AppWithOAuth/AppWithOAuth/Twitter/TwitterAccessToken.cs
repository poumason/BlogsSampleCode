using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWithOAuth.Twitter
{
    public class TwitterAccessToken
    {
        public String screen_name { get; set; }

        public String user_id { get; set; }

        public String oauth_token { get; set; }

        public String oauth_token_secret { get; set; }
    }
    
    public class UserProfile
    {
        public int id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public Entities entities { get; set; }
        public bool _protected { get; set; }
        public int followers_count { get; set; }
        public int friends_count { get; set; }
        public int listed_count { get; set; }
        public string created_at { get; set; }
        public int favourites_count { get; set; }
        public int utc_offset { get; set; }
        public string time_zone { get; set; }
        public bool geo_enabled { get; set; }
        public bool verified { get; set; }
        public int statuses_count { get; set; }
        public string lang { get; set; }
        public Status status { get; set; }
        public bool contributors_enabled { get; set; }
        public bool is_translator { get; set; }
        public bool is_translation_enabled { get; set; }
        public string profile_background_color { get; set; }
        public string profile_background_image_url { get; set; }
        public string profile_background_image_url_https { get; set; }
        public bool profile_background_tile { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public string profile_link_color { get; set; }
        public string profile_sidebar_border_color { get; set; }
        public string profile_sidebar_fill_color { get; set; }
        public string profile_text_color { get; set; }
        public bool profile_use_background_image { get; set; }
        public bool has_extended_profile { get; set; }
        public bool default_profile { get; set; }
        public bool default_profile_image { get; set; }
        public bool following { get; set; }
        public bool follow_request_sent { get; set; }
        public bool notifications { get; set; }
    }

    public class Entities
    {
        public Url url { get; set; }
        public Description description { get; set; }
    }

    public class Url
    {
        public Url1[] urls { get; set; }
    }

    public class Url1
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public int[] indices { get; set; }
    }

    public class Description
    {
        public object[] urls { get; set; }
    }

    public class Status
    {
        public string created_at { get; set; }
        public long id { get; set; }
        public string id_str { get; set; }
        public string text { get; set; }
        public string source { get; set; }
        public bool truncated { get; set; }
        public long in_reply_to_status_id { get; set; }
        public string in_reply_to_status_id_str { get; set; }
        public int in_reply_to_user_id { get; set; }
        public string in_reply_to_user_id_str { get; set; }
        public string in_reply_to_screen_name { get; set; }
        public object geo { get; set; }
        public object coordinates { get; set; }
        public object place { get; set; }
        public object contributors { get; set; }
        public int retweet_count { get; set; }
        public int favorite_count { get; set; }
        public Entities1 entities { get; set; }
        public bool favorited { get; set; }
        public bool retweeted { get; set; }
        public string lang { get; set; }
    }

    public class Entities1
    {
        public object[] hashtags { get; set; }
        public object[] symbols { get; set; }
        public User_Mentions[] user_mentions { get; set; }
        public object[] urls { get; set; }
    }

    public class User_Mentions
    {
        public string screen_name { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string id_str { get; set; }
        public int[] indices { get; set; }
    }
}