using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace EmotionAPISample.Utility
{
    public static class UtilityHelper
    {
        public static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);
            bitmapImage.SetSource(stream);
            stream.Dispose();
            return bitmapImage;
        }

        public static byte[] StreamToByteAraray(Stream stream)
        {
            byte[] byteArary = new byte[stream.Length];
            stream.Read(byteArary, 0, byteArary.Length);
            return byteArary;
        }
    }
}