using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FileAccessSample
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

        private async void OnCheckSDcardExist(object sender, RoutedEventArgs e)
        {
            if (await CheckHasExternalStorage())
            {
                txtLog.Text = "SD card existed.";
            }
            else
            {
                txtLog.Text = "Not detect any SD card.";
            }
        }

        private async void OnCheckSDCardId(object sender, RoutedEventArgs e)
        {
            await CheckSDCardId();
        }

        private async void OnAddFolderToFutureAccessList(object sender, RoutedEventArgs e)
        {
            await GetFolderFromPicker();
        }

        public static async Task<bool> CheckHasExternalStorage()
        {
            try
            {
                // 取得 logic root external folder
                StorageFolder externalDevices = KnownFolders.RemovableDevices;

                // 取得可用的 folders, 如果有 SD card 預設是第一個，
                // 但是 Desktop 可以接上多個 external storages 所以用 Count 檢查
                var folders = await externalDevices.GetFoldersAsync();

                return folders.Count > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> CheckSDCardId()
        {
            StorageFolder externalDevices = KnownFolders.RemovableDevices;

            // 取得 SD card
            StorageFolder sdCard = (await externalDevices.GetFoldersAsync()).FirstOrDefault();

            if (sdCard != null)
            {
                // 取得 ExternalStorageId        
                var allProperties = sdCard.Properties;
                
                IEnumerable<string> propertiesToRetrieve = new List<string> { "WindowsPhone.ExternalStorageId" , "WindowsMobile.ExternalStorageId" };

                var storageIdProperties = await allProperties.RetrievePropertiesAsync(propertiesToRetrieve);

                string cardId = (string)storageIdProperties["WindowsPhone.ExternalStorageId"];

                
            }

            return true;
        }

        public static async Task<StorageFolder> GetFolderFromPicker()
        {
            StorageFolder folder = null;
            try
            {
                // 最大上限 1000, 要自己手動管理
                RemoveOldestFolderOfAccessList();

                var folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                folderPicker.FileTypeFilter.Add("*");
                folder = await folderPicker.PickSingleFolderAsync();

                if (folder != null)
                {
                    // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                    // 相同目錄拿到的 token 是一樣的
                    string token = StorageApplicationPermissions.FutureAccessList.Add(folder, DateTime.UtcNow.Ticks.ToString());
                }
            }
            catch (Exception)
            {
            }

            return folder;
        }

        private static void RemoveOldestFolderOfAccessList(int removeCount = 2)
        {
            // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh972344(v=win.10)
            if (StorageApplicationPermissions.FutureAccessList.Entries.Count >= StorageApplicationPermissions.FutureAccessList.MaximumItemsAllowed)
            {
                var removeItems = StorageApplicationPermissions.FutureAccessList.Entries.Select(x =>
                {
                    long ticks = DateTime.UtcNow.Ticks;
                    long.TryParse(x.Metadata, out ticks);
                    return new Tuple<string, long>(x.Token, ticks);
                }).OrderBy(x => x).Take(removeCount);

                foreach (var item in removeItems)
                {
                    StorageApplicationPermissions.FutureAccessList.Remove(item.Item1);
                }
            }
        }
    }
}
