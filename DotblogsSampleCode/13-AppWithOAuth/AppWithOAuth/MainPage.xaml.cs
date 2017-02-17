using AppWithOAuth.Google;
using AppWithOAuth.LINE;
using AppWithOAuth.Twitter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppWithOAuth
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void OnGoogleSignInAPIClick(object sender, RoutedEventArgs e)
        {
            // 請求取得 access token
            var json = await GoogleSignInAPI.InvokeGoogleSignIn();
            // 轉換成 json 物件
            var accessToken = new GoogleAccessToken(json);
            // 取得 user profile
            var userProfile = await GoogleSignInAPI.GetUserInfo(accessToken.access_token);

            txtGoogleSignInResult.Text = userProfile.name + "\n" + userProfile.email;
            var bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri(userProfile.picture);
            imgPicture.Source = bitmapImage;
            txtGoogleSignInResult.Visibility = imgPicture.Visibility = Visibility.Visible;
        }

        private async void OnTwitterSignInAPIClick(object sender, RoutedEventArgs e)
        {
            var requestToken = await TwitterOAuthAPI.InvokeTwitterLogin();
            // 3. get oatuh_token and oauth_verifier
            requestToken = requestToken.Substring(requestToken.IndexOf("?") + 1);
            String[] data = requestToken.Split(new String[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            String token = data[0].Replace("oauth_token=", "");
            String verifier = data[1].Replace("oauth_verifier=", "");

            // 4. get access token
            TwitterAccessToken accessToken = await TwitterOAuthAPI.GetAccessToken(token, verifier);

            // 5. get user profile
            String json = await TwitterOAuthAPI.GetUserInfo(accessToken, verifier);

            // convert to object
            DataContractJsonSerializer tJsonSerial = new DataContractJsonSerializer(typeof(Twitter.UserProfile));
            MemoryStream tMS = new MemoryStream(Encoding.UTF8.GetBytes(json));
            Twitter.UserProfile user = tJsonSerial.ReadObject(tMS) as Twitter.UserProfile;
            txtTwitterResult.Text = String.Format("name: {0}, screen_name: {1}", user.name, user.screen_name);
            var bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri(user.profile_image_url);
            imgTwitter.Source = bitmapImage;
            txtTwitterResult.Visibility = imgTwitter.Visibility = Visibility.Visible;
        }

        private async void OnLINESignInAPIClick(object sender, RoutedEventArgs e)
        {
            // 1. request get AccessToken
            var accessTokenJson = await LINELoginAPI.RequestLoginAsync();

            // 2. convert string to AccessToken object
            DataContractJsonSerializer tJsonSerial = new DataContractJsonSerializer(typeof(LINEAccessToken));
            MemoryStream tMS = new MemoryStream(Encoding.UTF8.GetBytes(accessTokenJson));
            LINEAccessToken accesstoken = tJsonSerial.ReadObject(tMS) as LINEAccessToken;           
        }
    }
}