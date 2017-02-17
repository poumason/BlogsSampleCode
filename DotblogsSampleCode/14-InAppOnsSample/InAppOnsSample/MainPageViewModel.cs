using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InAppOnsSample
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event EventHandler<Exception> Errored;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<UserDataWrapper> Users { get; set; }

        public ObservableCollection<StoreProductDataWrapper> Products { get; set; }

        public ObservableCollection<StoreProductDataWrapper> PurchasedProducts { get; set; }

        private string message;
        public string Message
        {
            get { return message; }
            set { message = value;
                NotifyPropertyChanged(nameof(Message));
            }
        }

        private StoreContext storeContext { get; set; }

        public MainPageViewModel()
        {
            Users = new ObservableCollection<UserDataWrapper>();
            Products = new ObservableCollection<StoreProductDataWrapper>();
            PurchasedProducts = new ObservableCollection<StoreProductDataWrapper>();
        }

        public async Task Initialize()
        {
            // Get users
            var users = await Windows.System.User.FindAllAsync();

            int i = 1;
            foreach (var item in users)
            {
                this.Users.Add(new UserDataWrapper(item, $"user{i}"));
                i++;
            }
        }

        public void SetUserStoreContext(object sender, SelectionChangedEventArgs e)
        {
            ComboBox control = sender as ComboBox;

            if (control == null)
            {
                return;
            }

            UserDataWrapper user = control.SelectedItem as UserDataWrapper;
            storeContext = null;
            storeContext = StoreContext.GetForUser(user.Self);
        }

        public async Task GetAllLicense(object sender, RoutedEventArgs e)
        {
            var license = await storeContext.GetAppLicenseAsync();
        }

        public async Task GetProducts(object sender, RoutedEventArgs e)
        {
            var products = await storeContext.GetAssociatedStoreProductsAsync(GetProductKinds());

            if (products.ExtendedError != null)
            {
                Errored?.Invoke(this, products.ExtendedError);
                return;
            }

            Products.Clear();

            int count = products.Products.Count;

            foreach (var item in products.Products)
            {
                Products.Add(new StoreProductDataWrapper(item.Key, item.Value));
            }
        }

        public async Task GetPurchasedProducts(object sender, RoutedEventArgs e)
        {
            // 抓取已經購買的項目
            var purchased = await storeContext.GetUserCollectionAsync(GetProductKinds());

            if (purchased.ExtendedError != null)
            {
                Errored?.Invoke(this, purchased.ExtendedError);
                return;
            }

            if (purchased.Products == null || purchased.Products.Count == 0)
            {
                Message = "Not purchased any product";
                return;
            }

            PurchasedProducts.Clear();
            
            foreach (var item in purchased.Products)
            {
                PurchasedProducts.Add(new StoreProductDataWrapper(item.Key, item.Value));
            }
        }

        public async void PurchaseProduct(object sender, RoutedEventArgs e)
        {
            var first = Products.FirstOrDefault();

            GetConsumableBalanceRemainingAsync(first.Product.StoreId);

            if (first != null && first.Product != null)
            {
                // 購買產品
                var product = first.Product;
                if (product.IsInUserCollection == false)
                {
                    var result = await product.RequestPurchaseAsync();

                    if (result.ExtendedError != null)
                    {
                        Errored?.Invoke(this, result.ExtendedError);
                        return;
                    }

                    switch (result.Status) 
                    {
                        case StorePurchaseStatus.Succeeded:
                            // purchase successed
                            break;
                        case StorePurchaseStatus.AlreadyPurchased:
                            break;
                        case StorePurchaseStatus.NetworkError:
                        case StorePurchaseStatus.NotPurchased:
                        case StorePurchaseStatus.ServerError:
                            break;
                    }
                }
            }
        }

        public async void GetConsumableBalanceRemainingAsync(string storeId)
        {
            var result = await storeContext.GetConsumableBalanceRemainingAsync(storeId);

            if (result.ExtendedError != null)
            {
                Errored?.Invoke(this, result.ExtendedError);
                return;
            }
        }

        private string[] GetProductKinds()
        {
            /*
             *  ProductKind:
             *    Application,
             *    Game,
             *    Consumable, (Store-managed)
             *    UnmanagedConsumable, (Developer-managed)
             *    Durable
             */
            string[] filter = new string[] { "Durable", "Consumable" };
            return filter;
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}