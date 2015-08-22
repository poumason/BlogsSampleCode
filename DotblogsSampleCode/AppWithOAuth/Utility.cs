using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWithOAuth
{
    public class Utility
    {
        public static Dictionary<String, String> StringToDictionary(String param)
        {
            try
            {
                Dictionary<String, String> returnData = param.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                                                               .Select(part => part.Split('='))
                                                               .ToDictionary(split => split[0], split => split[1]);
                return returnData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
