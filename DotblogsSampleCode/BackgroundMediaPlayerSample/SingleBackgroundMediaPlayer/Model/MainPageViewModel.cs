using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Core;

namespace SingleBackgroundMediaPlayer
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MediaPlaybackItemDataWrapper> PlaybackList { get; private set; }

        public MediaPlaybackList MediaPlaybackList { get; private set; }

        public event EventHandler<MediaPlaybackItemDataWrapper> PlayingItemChanged;

        public CoreDispatcher UIDispatcher { get; set; }

        public MainPageViewModel()
        {
            PlaybackList = new ObservableCollection<MediaPlaybackItemDataWrapper>();
            MediaPlaybackList = new MediaPlaybackList();
            MediaPlaybackList.AutoRepeatEnabled = true;
            BuildMediaPlaybackList();

            MediaPlaybackList.CurrentItemChanged += MediaPlaybackList_CurrentItemChanged;
        }

        private void BuildMediaPlaybackList()
        {
            for (int i = 1; i < 5; i++)
            {
                string file = $"ms-appx:///Assets/mp3/0{i}.mp3";

                MediaSource source = MediaSource.CreateFromUri(new Uri(file, UriKind.RelativeOrAbsolute));
                MediaPlaybackItem item = new MediaPlaybackItem(source);
                MediaItemDisplayProperties displayProperty = item.GetDisplayProperties();
                displayProperty.Type = MediaPlaybackType.Music;
                displayProperty.MusicProperties.Title = $"0{i}.mp3";
                displayProperty.MusicProperties.AlbumArtist = "JJ";
                displayProperty.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri($"ms-appx:///Assets/mp3/0{i}.jpg", UriKind.RelativeOrAbsolute));
                item.ApplyDisplayProperties(displayProperty);

                PlaybackList.Add(new MediaPlaybackItemDataWrapper(item));
                MediaPlaybackList.Items.Add(item);
            }
        }

        private async void MediaPlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            await UIDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
           {
               var newItem = PlaybackList.Where(x => x.SourceObject == args.NewItem).FirstOrDefault();
               var oldItem = PlaybackList.Where(x => x.SourceObject == args.OldItem).FirstOrDefault();

               if (newItem != null)
               {
                   newItem.State = "Playing";
                   PlayingItemChanged?.Invoke(null, newItem);
               }

               if (oldItem != null)
               {
                   oldItem.State = string.Empty;
               }
           });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}