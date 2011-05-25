using System;

namespace Torshify
{
    public class TracksRemovedEventArgs : EventArgs
    {
        public int[] TrackIndices { get; private set; }

        public TracksRemovedEventArgs(int[] trackIndices)
        {
            TrackIndices = trackIndices;
        }
    }
}