using System;

namespace Torshify
{
    public class TracksAddedEventArgs : EventArgs
    {
        #region Constructors

        public TracksAddedEventArgs(int[] trackIndices, ITrack[] tracks)
        {
            TrackIndices = trackIndices;
            Tracks = tracks;
        }

        #endregion Constructors

        #region Properties

        public int[] TrackIndices
        {
            get; private set;
        }

        public ITrack[] Tracks
        {
            get; private set;
        }

        #endregion Properties
    }
}