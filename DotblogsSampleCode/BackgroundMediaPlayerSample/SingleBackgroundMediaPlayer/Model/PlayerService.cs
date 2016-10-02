using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace SingleBackgroundMediaPlayer
{
    public class PlayerService : IDisposable
    {
        public event EventHandler<MediaPlaybackState> StateChanged;

        private static PlayerService instance;

        private static object lockObject = new object();

        public static PlayerService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlayerService();
                }

                return instance;
            }
        }

        public MediaPlayer Player { get; private set; }

        public MediaPlaybackList PlaybackList
        {
            get
            {
                return Player.Source as MediaPlaybackList;
            }
            set
            {
                Player.Source = value;
            }
        }

        public MediaPlaybackState CurrentState { get; set; } = default(MediaPlaybackState);

        public PlayerService()
        {
            Player = new MediaPlayer();
            Player.AutoPlay = false;

            Player.CommandManager.IsEnabled = true;
            Player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            Player.CommandManager.PauseReceived += CommandManager_PauseReceived;
            Player.CommandManager.PlayReceived += CommandManager_PlayReceived;
            Player.CommandManager.NextReceived += CommandManager_NextReceived;
            Player.CommandManager.PreviousReceived += CommandManager_PreviousReceived;
        }

        private void CommandManager_PreviousReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerPreviousReceivedEventArgs args)
        {
            Previous();
        }

        private void CommandManager_NextReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerNextReceivedEventArgs args)
        {
            Next();
        }

        internal void Previous()
        {
            var playbackList = Player.Source as MediaPlaybackList;

            if (playbackList == null)
            {
                return;
            }

            playbackList.MovePrevious();
        }

        internal void Next()
        {
            var playbackList = Player.Source as MediaPlaybackList;

            if (playbackList == null)
            {
                return;
            }

            playbackList.MoveNext();
        }

        internal void Play()
        {
            Player.Play();
        }

        internal void Pause()
        {
            Player.Pause();
        }

        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            CurrentState = sender.PlaybackState;
            StateChanged?.Invoke(this, CurrentState);
        }

        private void CommandManager_PauseReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerPauseReceivedEventArgs args)
        {
            Pause();
        }

        private void CommandManager_PlayReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerPlayReceivedEventArgs args)
        {
            Play();
        }

        public void Dispose()
        {
            PlaybackList = null;
            Player = null;
            instance = null;
        }
    }
}