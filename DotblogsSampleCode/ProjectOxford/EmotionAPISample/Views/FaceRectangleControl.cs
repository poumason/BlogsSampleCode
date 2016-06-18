using EmotionAPISample.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace EmotionAPISample.Views
{
    public class FaceRectangleControl : Control
    {
        private EmotionData emotionData;

        private Flyout flyout;

        public FaceRectangleControl()
        {
            DataContextChanged += FaceRectangleControl_DataContextChanged;
            Unloaded += FaceRectangleControl_Unloaded;
            PointerEntered += FaceRectangleControl_PointerEntered;
            PointerExited += FaceRectangleControl_PointerExited;
        }

        private void FaceRectangleControl_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        }

        private void FaceRectangleControl_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            flyout?.ShowAt(this);
        }

        private void FaceRectangleControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DataContextChanged -= FaceRectangleControl_DataContextChanged;
            Unloaded -= FaceRectangleControl_Unloaded;
            PointerEntered -= FaceRectangleControl_PointerEntered;
            PointerExited -= FaceRectangleControl_PointerExited;
            emotionData = null;
        }

        private void FaceRectangleControl_DataContextChanged(Windows.UI.Xaml.FrameworkElement sender, Windows.UI.Xaml.DataContextChangedEventArgs args)
        {
            emotionData = DataContext as EmotionData;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (emotionData != null)
            {
                flyout = new Flyout();
                //TextBlock text = new TextBlock();
                //text.Text = $"anger: {emotionData.Scores.Anger}" +
                //            $"\ncontempt: {emotionData.Scores.Contempt}" +
                //            $"\ndisgust: {emotionData.Scores.Disgust}" +
                //            $"\nfear: {emotionData.Scores.Fear}" +
                //            $"\nhappiness: {emotionData.Scores.Happiness}" +
                //            $"\nneutral: {emotionData.Scores.Neutral}" +
                //            $"\nsadness: {emotionData.Scores.Sadness}" +
                //            $"\nsurprise: {emotionData.Scores.Surprise}";
                //text.TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap;
                //flyout.Content = text;
                flyout.Content = GetScoresStackPanel();
                Canvas.SetLeft(this, emotionData.FaceRectangle.Left);
                Canvas.SetTop(this, emotionData.FaceRectangle.Top);
            }
        }


        private StackPanel GetScoresStackPanel()
        {
            Dictionary<string, float> scores = new Dictionary<string, float>();
            scores.Add("anger", emotionData.Scores.Anger);
            scores.Add("contempt", emotionData.Scores.Contempt);
            scores.Add("disgust", emotionData.Scores.Disgust);
            scores.Add("fear", emotionData.Scores.Fear);
            scores.Add("happiness", emotionData.Scores.Happiness);
            scores.Add("neutral", emotionData.Scores.Neutral);
            scores.Add("sadness", emotionData.Scores.Sadness);
            scores.Add("surprise", emotionData.Scores.Surprise);

            var desc = scores.OrderByDescending(x => x.Value).ToList();

            int i = 0;

            StackPanel panel = new StackPanel();
            foreach (var item in desc)
            {
                TextBlock text = new TextBlock();
                text.Text = $"{item.Key}: {item.Value}";
                if (i == 0)
                {
                    text.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                    text.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                    i++;
                    StackPanel root = new StackPanel();
                    root.Orientation = Orientation.Horizontal;
                    Button btn = new Button();
                    btn.Content = new SymbolIcon(Symbol.Go);
                    btn.Tag = item.Key;
                    btn.Click += Btn_Click;
                    root.Children.Add(text);
                    root.Children.Add(btn);
                    panel.Children.Add(root);
                }
                else
                {
                    panel.Children.Add(text);
                }
            }

            return panel;
        }

        private async void Btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string searchKey = string.Empty;

            switch (btn.Tag.ToString())
            {
                case "happiness":
                    searchKey = "快樂";
                    break;
                case "anger":
                    searchKey = "生氣";
                    break;
                case "surprise":
                    searchKey = "驚喜";
                    break;
                case "sadness":
                    searchKey = "快樂";
                    break;
                case "neutral":
                    searchKey = "自由";
                    break;
                case "fear":
                    searchKey = "害怕";
                    break;
                case "disgust":
                    searchKey = "討厭";
                    break;
                case "contempt":
                    searchKey = "不屑";
                    break;
            }
        }
    }
}
