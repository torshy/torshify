using System;

namespace Torshify
{
    public class TracksRemovedEventArgs : EventArgs
    {
        #region Constructors

        public TracksRemovedEventArgs(int[] trackIndices)
        {
            TrackIndices = trackIndices;
        }

        #endregion Constructors

        #region Properties

        public int[] TrackIndices
        {
            get; private set;
        }

        #endregion Properties
    }
}