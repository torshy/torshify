using System;

namespace Torshify
{
    public interface IPlaylistTrack : ITrack
    {
        DateTime CreateTime { get; }
        //IUser Creator { get; }
        bool Seen { get; }
        IPlaylist Playlist { get; }
    }
}
