using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWithOAuth.LINE
{
    public class ProfileData
    {
        public string userId { get; set; }
        public string displayName { get; set; }
        /// <summary>
        /// default size
        /// </summary>
        public string pictureUrl { get; set; }

        /// <summary>
        /// 200x200
        /// </summary>
        public string GetLargePicture
        {
            get
            {
                if (string.IsNullOrEmpty(pictureUrl))
                {
                    return string.Empty;
                }
                else
                {
                    return $"{pictureUrl}/large";
                }
            }
        }

        /// <summary>
        /// 51x51
        /// </summary>
        public string GetSmallPicture
        {
            get
            {
                if (string.IsNullOrEmpty(pictureUrl))
                {
                    return string.Empty;
                }
                else
                {
                    return $"{pictureUrl}/small";
                }
            }
        }
    }
}