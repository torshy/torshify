using System;

namespace Torshify
{
    public class PlaylistMovedEventArgs : EventArgs
    {
        #region Constructors

        public PlaylistMovedEventArgs(IPlaylist playlist, int oldIndex, int newIndex)
        {
            Playlist = playlist;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        #endregion Constructors

        #region Properties

        public IPlaylist Playlist
        {
            get; private set;
        }

        public int OldIndex
        {
            get; private set;
        }

        public int NewIndex
        {
            get; private set;
        }

        #endregion Properties
    }
}