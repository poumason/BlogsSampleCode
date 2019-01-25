using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SingleBackgroundMediaPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainPageViewModel();
            ViewModel.UIDispatcher = Dispatcher;
            ViewModel.PlayingItemChanged += ViewModel_PlayingItemChanged;
            DataContext = ViewModel;
        }

        private void ViewModel_PlayingItemChanged(object sender, MediaPlaybackItemDataWrapper e)
        {
            PlaybackListListView.SelectedItem = e;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerService.Instance.Previous();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerService.Instance.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerService.Instance.Pause();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerService.Instance.Next();
        }

        private void PlaybackListListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selected = e.ClickedItem as MediaPlaybackItemDataWrapper;

            ViewModel.MediaPlaybackList.StartingItem = selected.SourceObject;
            PlayerService.Instance.PlaybackList = ViewModel.MediaPlaybackList;
            PlayerService.Instance.Play();
        }

        private void DoubleMediaPlayer()
        {
            var list = ViewModel.MediaPlaybackList;
            var list1 = new MediaPlaybackList();
            var list2 = new MediaPlaybackList();

            for (int i = 0; i < list.Items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    list1.Items.Add(list.Items[i]);
                }
                else
                {
                    list2.Items.Add(list.Items[i]);
                }
            }

            player1.PlaybackList = list1;
            player2.PlaybackList = list2;

            player1.Play();
            player2.Play();
        }

        PlayerService player1 = new PlayerService();
        PlayerService player2 = new PlayerService();
    }
}