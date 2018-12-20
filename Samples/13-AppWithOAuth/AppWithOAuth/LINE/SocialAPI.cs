using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppWithOAuth.LINE
{
    public class SocialAPI
    {
        public static async Task<ProfileData> GetProfile(string accessToken)
        {
            using (HttpClient client = new HttpClient())
            {
                // https://api.line.me/v2/profile 設定 Authentication header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                string jsonResult = await client.GetStringAsync(new Uri("https://api.line.me/v2/profile"));

                DataContractJsonSerializer tJsonSerial = new DataContractJsonSerializer(typeof(ProfileData));
                MemoryStream tMS = new MemoryStream(Encoding.UTF8.GetBytes(jsonResult));
                ProfileData data = tJsonSerial.ReadObject(tMS) as ProfileData;

                return data;
            }
        }
    }
}