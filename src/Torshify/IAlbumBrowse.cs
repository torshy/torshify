using System;

namespace Torshify
{
    public interface IAlbumBrowse : ISessionObject
    {
        #region Events

        event EventHandler<UserDataEventArgs> Completed;

        #endregion Events

        #region Properties

        IAlbum Album
        {
            get;
        }

        IArtist Artist
        {
            get;
        }

        IArray<string> Copyrights
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

        string Review
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