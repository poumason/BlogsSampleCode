using BusSearchModel.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace BusSearchModel
{
    public class SearchService
    {
        public SearchService()
        {

        }

        public async Task<List<BusData>> SearchBusList()
        {
            var folder = await Package.Current.InstalledLocation.GetFolderAsync("Data");
            var file = await folder.GetFileAsync("BusList.txt");
            String content = String.Empty;
            using (StreamReader reader = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                content = reader.ReadToEnd();
            }
            List<BusData> busSearchResult = new List<BusData>();
            String[] temp = content.Split('|');
            foreach (var item in temp)
            {
                if (String.IsNullOrEmpty(item) == false)
                {
                    busSearchResult.Add(new BusData(item));
                }
            }
            return busSearchResult;
        }

        public async Task<List<BusData>> SearchBusWithLocation()
        {
            // get geo location
            var location = await GetMyLocation();
            if (location!= null)
            {
                var searchData = await RequestGetBusList(location);
                if (String.IsNullOrEmpty(searchData) == false)
                {
                    List<BusData> busSearchResult = new List<BusData>();
                    String[] temp = searchData.Split('|');
                    foreach (var item in temp)
                    {
                        busSearchResult.Add(new BusData(item));
                    }
                    return busSearchResult;
                }
            }
            return null;
        }

        private async Task<LocationData> GetMyLocation()
        {
            // Get cancellation token
            CancellationTokenSource _cts = new CancellationTokenSource();
            try
            {
                var accessStatus = await Geolocator.RequestAccessAsync();

                if (accessStatus == GeolocationAccessStatus.Allowed)
                {

                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 500 };

                    // Carry out the operation
                    var _pos = await geolocator.GetGeopositionAsync().AsTask(_cts.Token);

                    return new LocationData()
                    {
                        Lat = (float)_pos.Coordinate.Point.Position.Latitude,
                        Lon = (float)_pos.Coordinate.Point.Position.Longitude,
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (TaskCanceledException tce)
            {
                throw new Exception("Task cancelled" + tce.Message);
            }
            finally
            {
                _cts = null;
            }
        }

        private async Task<String> RequestGetBusList(LocationData location)
        {
            CancellationTokenSource canel = new CancellationTokenSource();
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Referer", "http://www.5284.com.tw/Dybus.aspx?Lang=#");                   

                Dictionary<String, String> post1 = new Dictionary<string, string> {
                    {"ACTION","101" }, {"Gbtn3S","%25E5%259C%25B0%25E5%259C%2596%25E5%25AE%259A%25E4%25BD%258D"},
                    {"lang","cht" }
                };
                FormUrlEncodedContent postContent1 = new FormUrlEncodedContent(post1);
                var request1 = GetSearchRequest(postContent1, "http://www.5284.com.tw/Aspx/dybus/arrivalInfo.aspx");

                var x = await client.SendAsync(request1, canel.Token);
                
                Dictionary<String, String> post2 = new Dictionary<string, string> {
                    {"ACTION","337" }, {"Groupx",location.Lat.ToString()},
                    {"Groupy",location.Lon.ToString()}, {"lang","cht" }
                };
                FormUrlEncodedContent postContent2 = new FormUrlEncodedContent(post2);
                var request2 = GetSearchRequest(postContent2, "http://www.5284.com.tw/Aspx/dybus/arrivalInfo.aspx");

                var result =await client.SendAsync(request2, canel.Token);

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    String resultData = await result.Content.ReadAsStringAsync();
                    return resultData;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                canel = null;
            }
            return null;
        }

        private HttpRequestMessage GetSearchRequest(HttpContent content, String uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Version = new Version("1.1");
            request.Content = content;
            return request;
        }
    }
}
