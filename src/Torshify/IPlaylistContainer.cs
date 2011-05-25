using System;

namespace Torshify
{
    public interface IPlaylistContainer : ISessionObject
    {
        IEditableArray<IContainerPlaylist> Playlists { get; }
        IUser Owner { get; }
        bool IsLoaded { get; }
        event EventHandler Loaded;
        event EventHandler<PlaylistEventArgs> PlaylistAdded;
        event EventHandler<PlaylistMovedEventArgs> PlaylistMoved;
        event EventHandler<PlaylistEventArgs> PlaylistRemoved;
    }
}