using System;

namespace Torshify
{
    public class TrackCreatedChangedEventArgs : EventArgs
    {
        #region Constructors

        public TrackCreatedChangedEventArgs(ITrack track, DateTime when)
        {
            Track = track;
            When = when;
        }

        #endregion Constructors

        #region Properties

        public ITrack Track
        {
            get; private set;
        }

        public DateTime When
        {
            get; private set;
        }

        #endregion Properties
    }
}