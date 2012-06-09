using System;

namespace Torshify
{
    public interface IPlaylistTrack : ITrack
    {
        DateTime CreateTime
        {
            get;
        }

        bool Seen
        {
            get;
        }

        IPlaylist Playlist
        {
            get;
        }
    }
}
