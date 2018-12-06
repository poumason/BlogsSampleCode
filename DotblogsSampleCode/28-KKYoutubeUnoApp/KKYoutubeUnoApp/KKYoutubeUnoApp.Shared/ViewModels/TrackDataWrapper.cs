using KKBOX.OpenAPI.ServiceModel;
using System.Linq;

namespace KKYoutubeUnoApp.ViewModels
{
    public class TrackDataWrapper
    {
        private TrackData source;

        public string AlbumUrl
        {
            get { return source?.Album?.Images?.FirstOrDefault()?.Url; }
        }

        public string SongName
        {
            get { return source?.Name; }
        }

        public string ArtistWithAlbumName
        {
            get
            {
                var album = source?.Album;

                if (album == null)
                {
                    return string.Empty;
                }

                if (album.Artist == null)
                {
                    return album.Name;
                }

                return $"{album.Artist.Name} - {album.Name}";
            }
        }

        public TrackDataWrapper(TrackData data)
        {
            source = data;
        }
    }
}