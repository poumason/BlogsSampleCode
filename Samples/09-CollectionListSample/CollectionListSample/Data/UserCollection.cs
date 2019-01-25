using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace CollectionListSample
{
    public class UserCollection : ObservableCollection<UserData>, ISupportIncrementalLoading
    {
        public bool HasMoreItems
        {
            get;
            private set;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            // 實作載入更多資料的邏輯
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }

        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint count)
        {
            if (currentIndex >= 150)
            {
                HasMoreItems = false;
            }
            else
            {
                LoadData(currentIndex);
            }
            return new LoadMoreItemsResult { Count = (uint)currentIndex };
        }

        private void LoadData(int index)
        {
            int max = index + 50;

            for (int i = index; i < max; i++)
            {
                Add(new UserData { Name = $"Pou{i}" });
            }
            currentIndex = max;
        }

        private int currentIndex = 0;

        public UserCollection()
        {
            LoadData(currentIndex);
            HasMoreItems = true;
        }
    }
}