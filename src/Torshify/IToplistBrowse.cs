using System;

namespace Torshify
{
    public interface IToplistBrowse
    {
        #region Events

        event EventHandler<UserDataEventArgs> Completed;

        #endregion Events

        #region Properties

        IArray<IAlbum> Albums
        {
            get;
        }

        IArray<IArtist> Artists
        {
            get;
        }

        Error Error
        {
            get;
        }

        bool IsComplete
        {
            get;
        }

        bool IsLoaded
        {
            get;
        }

        IArray<ITrack> Tracks
        {
            get;
        }

        TimeSpan BackendRequestDuration
        {
            get;
        }

        #endregion Properties
    }
}