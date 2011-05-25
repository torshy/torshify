using System;

namespace Torshify
{
    public class TracksAddedEventArgs : EventArgs
    {
        public int[] TrackIndices { get; private set; }
        public ITrack[] Tracks { get; private set; }

        public TracksAddedEventArgs(int[] trackIndices, ITrack[] tracks)
        {
            TrackIndices = trackIndices;
            Tracks = tracks;
        }
    }
}