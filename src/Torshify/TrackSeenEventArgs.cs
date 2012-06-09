using System;

namespace Torshify
{
    public class TrackSeenEventArgs : EventArgs
    {
        #region Constructors

        public TrackSeenEventArgs(ITrack track, bool seen)
        {
            Track = track;
            IsSeen = seen;
        }

        #endregion Constructors

        #region Properties

        public ITrack Track
        {
            get; private set;
        }

        public bool IsSeen
        {
            get; private set;
        }

        #endregion Properties
    }
}