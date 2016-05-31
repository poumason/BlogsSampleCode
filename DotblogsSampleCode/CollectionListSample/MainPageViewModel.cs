using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionListSample
{
    public class MainPageViewModel
    {
        public UserCollection DataSource { get; set; }

        public MainPageViewModel()
        {
            DataSource = new UserCollection();           
        }
    }
}
