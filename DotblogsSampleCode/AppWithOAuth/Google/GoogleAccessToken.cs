using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppWithOAuth.Google
{
    public class GoogleAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }

        public GoogleAccessToken() { }

        public GoogleAccessToken(String json)
        {
            DataContractJsonSerializer tJsonSerial = new DataContractJsonSerializer(typeof(GoogleAccessToken));
            MemoryStream tMS = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var self = tJsonSerial.ReadObject(tMS) as GoogleAccessToken;
            access_token = self.access_token;
            token_type = self.token_type;
            expires_in = self.expires_in;
            refresh_token = self.refresh_token;
            id_token = self.id_token;
        }
    }

    public class UserProfile
    {
        public string id { get; set; }
        public string email { get; set; }
        public bool verified_email { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string picture { get; set; }
        public string gender { get; set; }
        public string locale { get; set; }

        public UserProfile() { }

        public UserProfile(String json)
        {
            DataContractJsonSerializer tJsonSerial = new DataContractJsonSerializer(typeof(UserProfile));
            MemoryStream tMS = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var self = tJsonSerial.ReadObject(tMS) as UserProfile;
            id = self.id;
            email = self.email;
            verified_email = self.verified_email;
            name = self.name;
            given_name = self.given_name;
            picture = self.picture;
            gender = self.gender;
            locale = self.locale;
        }
    }
}