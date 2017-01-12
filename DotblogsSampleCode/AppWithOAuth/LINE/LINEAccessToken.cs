using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWithOAuth.LINE
{
    public class LINEAccessToken
    {
        public string mid { get; set; }

        public string access_token { get; set; }

        public string token_type { get; set; }

        public int expires_in { get; set; }

        public string refresh_token { get; set; }

        public object scope { get; set; }
    }
}