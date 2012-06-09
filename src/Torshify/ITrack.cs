using System;

namespace Torshify
{
    public interface ITrack : ISessionObject
    {
        #region Properties

        IAlbum Album
        {
            get;
        }

        IArray<IArtist> Artists
        {
            get;
        }

        int Disc
        {
            get;
        }

        TimeSpan Duration
        {
            get;
        }

        Error Error
        {
            get;
        }

        int Index
        {
            get;
        }

        TrackAvailablity Availability
        {
            get;
        }

        TrackOfflineStatus OfflineStatus
        {
            get;
        }

        bool IsLoaded
        {
            get;
        }

        bool IsStarred
        {
            get;
            set;
        }

        bool IsLocal
        {
            get;
        }

        bool IsAutolinked
        {
            get;
        }

        string Name
        {
            get;
        }

        int Popularity
        {
            get;
        }

        ITrack AutolinkedTrack
        {
            get;
        }

        #endregion Properties
    }
}