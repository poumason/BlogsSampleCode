using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DevCenterAnalysisAPI
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string token = Application["access_token"]?.ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = GetDevCenterAccessToken();
                Application.Lock();
                Application.Add("access_token", token);
            }
            GetErrorReport(token);
            GetRateReport(token);
            GetReviewReport(token);
        }

        const string scope = "https://manage.devcenter.microsoft.com";

        private string GetDevCenterAccessToken()
        {
            string tenantId = WebConfigurationManager.AppSettings["TenantId"];
            string clientId = WebConfigurationManager.AppSettings["ClientId"];
            string clientSecret = WebConfigurationManager.AppSettings["ClientSecret"];

            string tokenEndpoint = string.Format("https://login.microsoftonline.com/{0}/oauth2/token", tenantId);
            using (WebClient client = new WebClient())
            {
                string param = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&resource={2}",
                                              clientId, clientSecret, scope);
                //application/x-www-form-urlencoded
                string json = client.UploadString(tokenEndpoint, param);
                AccessTokenObject result = JsonConvert.DeserializeObject<AccessTokenObject>(json);
                return result.AccessToken;
            }
        }

        private void GetErrorReport(string accessToken)
        {
            string url = "https://manage.devcenter.microsoft.com/v1.0/my/analytics/failurehits";
            string queryStr = "applicationId=" + "9nblggh5f25p" +
                              "&startDate=" + "2016/01/01" +
                              "&endDate=" + "2016/04/30" +
                              "&top=2&skip=0";
            using (WebClient client = new WebClient())
            {
                string link = string.Format("{0}?{1}", url, queryStr);
                client.Headers.Add("Authorization", string.Format("Bearer {0}", accessToken));
                string json = client.DownloadString(link);
            }
        }

        private void GetRateReport(string accessToken)
        {
            string url = "https://manage.devcenter.microsoft.com/v1.0/my/analytics/ratings";
            string queryStr = "applicationId=" + "9nblggh5f25p" +
                              "&startDate=" + "2016/01/01" +
                              "&endDate=" + "2016/04/30" +
                              "&top=2&skip=0" +
                              "&aggregationLevel=" + "month";
            using (WebClient client = new WebClient())
            {
                string link = string.Format("{0}?{1}", url, queryStr);
                client.Headers.Add("Authorization", string.Format("Bearer {0}", accessToken));
                string json = client.DownloadString(link);
            }
        }

        private void GetReviewReport(string accessToken)
        {
            string url = "https://manage.devcenter.microsoft.com/v1.0/my/analytics/reviews";
            string queryStr = "applicationId=" + "9nblggh5f25p" +
                              "&startDate=" + "2016/01/01" +
                              "&endDate=" + "2016/04/30" +
                              "&top=2&skip=0" +
                              "";
                              //"&filter=" + "contains(reviewText,'查詢') and contains(reviewText,'錯誤')";
            using (WebClient client = new WebClient())
            {
                string link = string.Format("{0}?{1}", url, queryStr);
                client.Headers.Add("Authorization", string.Format("Bearer {0}", accessToken));
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(link);
                var reviewData = JsonConvert.DeserializeObject<AnalysisData<ReviewData>>(json);                
            }
        }

        protected void OnGetAppAcquisitions_Click(object sender, EventArgs e)
        {
            string token = Application["access_token"]?.ToString();
            const string url = "https://manage.devcenter.microsoft.com/v1.0/my/analytics/appacquisitions";

            string queryStr = "applicationId=" + "9nblggh5f25p" +
                             "&startDate=" + "2016/11/01" +
                             "&endDate=" + "2016/11/30";
            //"&filter=" + "contains(reviewText,'查詢') and contains(reviewText,'錯誤')";
            using (WebClient client = new WebClient())
            {
                string link = string.Format("{0}?{1}", url, queryStr);
                client.Headers.Add("Authorization", string.Format("Bearer {0}", token));
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(link);
                var reviewData = JsonConvert.DeserializeObject<AnalysisData<ReviewData>>(json);
            }
        }
    }
}