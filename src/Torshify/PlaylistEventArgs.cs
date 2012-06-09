using System;

namespace Torshify
{
    public class PlaylistEventArgs : EventArgs
    {
        public PlaylistEventArgs(IPlaylist playlist, int position)
        {
            Playlist = playlist;
            Position = position;
        }

        public IPlaylist Playlist
        {
            get; 
            private set;
        }

        public int Position
        {
            get; 
            private set;
        }
    }
}