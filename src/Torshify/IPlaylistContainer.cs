using System;

namespace Torshify
{
    public interface IPlaylistContainer : ISessionObject
    {
        #region Events

        event EventHandler Loaded;

        event EventHandler<PlaylistEventArgs> PlaylistAdded;

        event EventHandler<PlaylistMovedEventArgs> PlaylistMoved;

        event EventHandler<PlaylistEventArgs> PlaylistRemoved;

        #endregion Events

        #region Properties

        bool IsLoaded
        {
            get;
        }

        IUser Owner
        {
            get;
        }

        IEditableArray<IContainerPlaylist> Playlists
        {
            get;
        }

        #endregion Properties
    }
}