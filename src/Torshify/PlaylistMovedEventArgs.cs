using System;

namespace Torshify
{
    public class PlaylistMovedEventArgs : EventArgs
    {
        public PlaylistMovedEventArgs(IPlaylist playlist, int oldIndex, int newIndex)
        {
            Playlist = playlist;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        public IPlaylist Playlist { get; private set; }
        public int OldIndex { get; private set; }
        public int NewIndex { get; private set; }
    }
}