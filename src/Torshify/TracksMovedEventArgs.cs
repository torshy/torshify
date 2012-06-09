using System;

namespace Torshify
{
    public class TracksMovedEventArgs : EventArgs
    {
        #region Constructors

        public TracksMovedEventArgs(int[] trackIndices, int newPosition)
        {
            TrackIndices = trackIndices;
            NewPosition = newPosition;
        }

        #endregion Constructors

        #region Properties

        public int[] TrackIndices
        {
            get; private set;
        }

        public int NewPosition
        {
            get; private set;
        }

        #endregion Properties
    }
}