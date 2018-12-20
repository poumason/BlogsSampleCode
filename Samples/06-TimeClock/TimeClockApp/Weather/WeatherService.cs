using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Web.Http;

namespace TimeClockApp
{
    public class WeatherService
    {
        const string OpenAPIKey = "CWB-FDB1D97E-5984-4F10-B7E2-574E7BA0B268";

        private HttpClient httpClient { get; set; }

        string UrlFormat = "http://opendata.cwb.gov.tw/opendataapi?dataid={0}&authorizationkey={1}";

        public WeatherService()
        {
            httpClient = new HttpClient();
        }

        public async void UpdateToday()
        {
            string url = string.Format(UrlFormat, "F-D0047-069", OpenAPIKey);
            string data = await httpClient.GetStringAsync(new Uri(url));
            StringReader rdr = new StringReader(data);
            XmlSerializer serializer = new XmlSerializer(typeof(cwbopendata));
            cwbopendata resultingMessage = (cwbopendata)serializer.Deserialize(rdr);
        }

        public void UpdateWeek()
        {

        }
    }
}