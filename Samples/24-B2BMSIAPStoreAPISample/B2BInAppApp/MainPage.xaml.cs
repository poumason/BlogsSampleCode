using B2BInAppService.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace B2BInAppApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private StoreContext storeContext = StoreContext.GetDefault();

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await GenerateMSStoreIDAndSyncToService();
        }

        private async Task GetAssociatedProductsButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a filtered list of the product AddOns I care about
            string[] filterList = new string[] { "Consumable", "Durable", "UnmanagedConsumable" };

            // Get list of Add Ons this app can sell, filtering for the types we know about
            StoreProductQueryResult addOns = await storeContext.GetAssociatedStoreProductsAsync(filterList);
        }

        private async void PurchaseAddOnButton_Click(object sender, RoutedEventArgs e)
        {
            string productStoreId = "";

            StorePurchaseResult result = await storeContext.RequestPurchaseAsync(productStoreId);

            if (result.ExtendedError != null)
            {
                Debug.WriteLine(result.ExtendedError);
                return;
            }

            switch (result.Status)
            {
                case StorePurchaseStatus.AlreadyPurchased:
                    break;
                case StorePurchaseStatus.Succeeded:
                    break;
                case StorePurchaseStatus.NotPurchased:
                    break;
                case StorePurchaseStatus.NetworkError:
                    break;
                case StorePurchaseStatus.ServerError:
                    break;
                default:
                    break;
            }

            var license = await storeContext.GetAppLicenseAsync();

            if (license.AddOnLicenses != null)
            {
                foreach (var licenseItem in license.AddOnLicenses)
                {
                    Debug.WriteLine($"{licenseItem.Key}:{licenseItem.Value.IsActive}:{licenseItem.Value.ExpirationDate.ToLocalTime()}");
                }
            }
        }

        private async Task GenerateMSStoreIDAndSyncToService()
        {
            // 1. get collection/purchase Azure AD access token from service
            var authResult = await GetTokenFromAzureOAuthAsync();

            // 2. generate MS Store ID by collection and purchase Azure AD access token
            string uid = "example@live.com";
            // publisherUserId is identify user on your server, such as: serial id, not Microsoft Account
            var collectionStoreId = await storeContext.GetCustomerCollectionsIdAsync(authResult.Collection, uid);
            var purchaseStoreId = await storeContext.GetCustomerPurchaseIdAsync(authResult.Purchase, uid);

            // 3. report MS Store ID to service
            var actionData = new PostActionData()
            {
                UID = uid,
                AuthData = authResult,
                CollectionStoreID = collectionStoreId,
                PurchaseStoreID = purchaseStoreId
            };

            HttpClient client = new HttpClient();
            var content = new HttpStringContent(actionData.Stringify());
            content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
            var result = await client.PostAsync(new Uri("http://localhost/api/values"), content);

            var responseContent = await result.Content.ReadAsStringAsync();
            Debug.WriteLine(responseContent);
        }

        private async Task<AccessTokenCollectionData> GetTokenFromAzureOAuthAsync()
        {
            HttpClient client = new HttpClient();
            var responseString = await client.GetStringAsync(new Uri("http://localhost/api/values"));
            JsonValue response = JsonValue.Parse(responseString);
            var jsonObject = response.GetObject();

            AccessTokenCollectionData data = new AccessTokenCollectionData
            {
                Auth = jsonObject.GetNamedString("Auth"),
                Collection = jsonObject.GetNamedString("Collection"),
                Purchase = jsonObject.GetNamedString("Purchase"),
            };

            return data;
        }
    }
}
