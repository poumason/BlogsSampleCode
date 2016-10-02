using System.ComponentModel;
using Windows.Media.Playback;

namespace SingleBackgroundMediaPlayer
{
    public class MediaPlaybackItemDataWrapper :INotifyPropertyChanged
    {
        public string Title
        {
            get { return SourceObject?.GetDisplayProperties().MusicProperties.Title; }
        }

        private string state;
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                RaisePropertyChanged(nameof(State));
            }
        }

        public MediaPlaybackItem SourceObject { get; private set; }

        public MediaPlaybackItemDataWrapper(MediaPlaybackItem source)
        {
            SourceObject = source;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
