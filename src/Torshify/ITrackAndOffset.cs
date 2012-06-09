using System;

namespace Torshify
{
    public interface ITrackAndOffset
    {
        ITrack Track
        {
            get;
        }

        TimeSpan Offset
        {
            get;
        }
    }
}
