using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace TimeClockApp
{
    public class ValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var defaultColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xfc, 0xfc, 0xfc));
            if (value.GetType() == typeof(string))
            {
                string percentStr = value.ToString();
                if (string.IsNullOrEmpty(percentStr) == false)
                {
                    double percentV = 0;
                    if (double.TryParse(percentStr, out percentV))
                    {
                        if (percentV < 50)
                        {
                            defaultColor = new SolidColorBrush(Colors.Red);
                        }
                    }
                }
            }
            return defaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}
