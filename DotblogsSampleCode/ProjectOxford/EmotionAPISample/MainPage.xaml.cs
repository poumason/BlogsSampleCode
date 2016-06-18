using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using EmotionAPISample.Data;
using EmotionAPISample.Views;
using Windows.UI.Popups;
using EmotionAPISample.Library;
using EmotionAPISample.Utility;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EmotionAPISample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MenuFlyout menuFlyout;
        private Stream fileStream;

        public MainPage()
        {
            this.InitializeComponent();

            PrepareFlyoutItesm();
        }

        private void OnCapturePhoto(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            menuFlyout.ShowAt(btn);
        }

        #region FlyoutItem 
        private void PrepareFlyoutItesm()
        {
            menuFlyout = new MenuFlyout();
            MenuFlyoutItem camItem = new MenuFlyoutItem();
            camItem.Text = "Camera";
            camItem.Tag = "camera";
            camItem.Click += MenuItem_Click;
            menuFlyout.Items.Add(camItem);
            MenuFlyoutItem phototem = new MenuFlyoutItem();
            phototem.Text = "Photo Library";
            phototem.Tag = "photo";
            phototem.Click += MenuItem_Click;
            menuFlyout.Items.Add(phototem);
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file;
            var item = sender as MenuFlyoutItem;
            if (item.Tag.ToString() == "photo")
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".png");

                file = await openPicker.PickSingleFileAsync();
            }
            else
            {
                CameraCaptureUI captureUI = new CameraCaptureUI();
                captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
                captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

                file = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            }
            if (file != null)
            {
                // Application now has read/write access to the picked file
                var img = await UtilityHelper.LoadImage(file);
                imgSource.Source = img;
                fileStream = await file.OpenStreamForWriteAsync();
            }
        }
        #endregion

     

        private async void OnAnalysisByEmotionAPI(object sender, RoutedEventArgs e)
        {
            string key = "";
            //file straem to byte array
            byte[] data = UtilityHelper.StreamToByteAraray(fileStream);
            EmotionAPIService api = new EmotionAPIService(key);
            var emoptionResult = await api.Recognize(data.AsBuffer(), string.Empty);
            if (emoptionResult == null)
            {
                MessageDialog dialog = new MessageDialog("failed");
                var task = dialog.ShowAsync();
            }
            else
            {
                AddFaceRectangles(emoptionResult);
            }
        }

        private string GetFaceRectangleQueryString(List<FacerectangleData> rectangles)
        {
            List<string> param = new List<string>();
            // override ToString method to return left,top,width,height 
            param.AddRange(rectangles.Select(x => x.ToString()).ToList());
            // Delimited multiple face rectangles with a “;”. 
            return $"faceRectangles={string.Join(";", param)}";
        }

        private void ClearBorder()
        {
            Image img = null;

            foreach (var item in gridCanvas.Children)
            {
                if (item is Image)
                {
                    img = item as Image;
                    break;
                }
            }
            gridCanvas.Children.Clear();
            gridCanvas.Children.Add(img);
        }

        private void AddFaceRectangles(List<EmotionData> dataList)
        {
            ClearBorder();
            foreach (var item in dataList)
            {
                FaceRectangleControl control = new FaceRectangleControl();
                control.DataContext = item;
                gridCanvas.Children.Add(control);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), null);
        }
    }
}