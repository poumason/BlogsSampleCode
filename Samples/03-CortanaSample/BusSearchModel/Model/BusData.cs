using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusSearchModel.Model
{
    /// <summary>
    /// 以公车资讯为例，当作可使用的 ViewModel
    /// </summary>
    public class BusData
    {
        public String Name { get; set; }

        public Double Lat { get; set; }

        public Double Lon { get; set; }

        public String GoId { get; set; }

        public String BackId { get; set; }

        public BusData(String param)
        {
            String[] paramValue = param.Split(',');
            if (paramValue.Length == 6)
            {
                Name = paramValue[0];
                Lat = Double.Parse(paramValue[1]);
                Lon = Double.Parse(paramValue[2]);
                GoId = paramValue[4];
                BackId = paramValue[5];
            }
        }
    }
}
