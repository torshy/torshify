using System;

namespace Torshify
{
    public class TracksMovedEventArgs : EventArgs
    {
        public int[] TrackIndices { get; private set; }
        public int NewPosition { get; private set; }

        public TracksMovedEventArgs(int[] trackIndices, int newPosition)
        {
            TrackIndices = trackIndices;
            NewPosition = newPosition;
        }
    }
}