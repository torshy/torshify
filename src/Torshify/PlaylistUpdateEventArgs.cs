using System;

namespace Torshify
{
    public class PlaylistUpdateEventArgs : EventArgs
    {
        #region Constructors

        public PlaylistUpdateEventArgs(bool done)
        {
            IsDone = done;
        }

        #endregion Constructors

        #region Properties

        public bool IsDone
        {
            get; private set;
        }

        #endregion Properties
    }
}