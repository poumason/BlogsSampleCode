using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.UserActivities;
using Windows.UI;
using Windows.UI.Shell;
using Windows.UI.Xaml;

namespace UserActivitySample
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string activityId;
        public string ActivityId
        {
            get { return activityId; }
            set
            {
                activityId = value;
                RaisePropertyChanged(nameof(ActivityId));
            }
        }

        private UserActivity currentActivity;
        public UserActivity CurrentActivity
        {
            get { return currentActivity; }
            set
            {
                currentActivity = value;
                RaisePropertyChanged(nameof(CurrentActivity));
            }
        }
        
        private UserActivitySession activitySesion;

        public MainPageViewModel()
        {
            ActivityId = "18a5e955-78fc-4a3a-bb5e-42b9f137b8fa";
        }

        public async void OnGetActivityClick(object sender, RoutedEventArgs e)
        {
            ActivityId = Guid.NewGuid().ToString();

            // Get or Create a activity
            var activity = await UserActivityChannel.GetDefault().GetOrCreateUserActivityAsync(ActivityId);

            // if you want to use Adaptive card, reference: https://docs.microsoft.com/en-us/adaptive-cards/create/gettingstarted
            activity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(@"{
                                              ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
                                              ""type"": ""AdaptiveCard"",
                                              ""backgroundImage"": ""https://cdn2.ettoday.net/images/1376/d1376700.jpg"",
                                              ""version"": ""1.0"",
                                              ""body"" : [ {
                                                ""type"": ""Container"", 
                                                ""items"": [
                                                    {
                                                        ""type"": ""TextBlock"",
                                                        ""text"": ""from adaptive card"",
                                                        ""size"": ""large"",
                                                        ""wrap"": ""true"",
                                                        ""maxLines"": ""3""
                                                    }    
                                                ]
                                              } ]
                                              }");

            if (activity.State == UserActivityState.New)
            {
                // this is a new activity
                activity.VisualElements.DisplayText = "new activity";
                activity.ActivationUri = new Uri($"testapp://mainPage?state=new&id={ActivityId}");
            }
            else
            {
                // this is published activity
                activity.VisualElements.DisplayText = "published activity";
                activity.ActivationUri = new Uri($"testapp://mainPage?state=published&id={ActivityId}");
            }

            // set activity content info
            activity.ContentInfo = UserActivityContentInfo.FromJson(@"{
                ""user_id"": ""pou"",
                ""email"": ""poumason@live.com""
            }");

            // FallbackUri is handled when System invoke ActivationUri failed.
            //activity.FallbackUri = new Uri("https://dotblogs.com.tw/pou");

            activity.VisualElements.BackgroundColor = Color.FromArgb(0xFF, 0x88, 0x88, 0x88);

            await activity.SaveAsync();

            // a activity match an session, need close other sessions.
            activitySesion?.Dispose();
            activitySesion = activity.CreateSession();
        }

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch
            {
                // nothing
            }
        }

        public void OnCloseSessionClick(object sender, RoutedEventArgs e)
        {
            activitySesion?.Dispose();
        }
    }
}