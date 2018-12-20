using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Store;

namespace InAppOnsSample
{
    public class StoreProductDataWrapper
    {
        public StoreProduct Product { get; set; }

        public string Title { get; set; }

        public StoreProductDataWrapper(string title, StoreProduct product)
        {
            Title = title;
            Product = product;
        }
    }
}
