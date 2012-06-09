using System;

namespace Torshify
{
    public interface IArtistBrowse : ISessionObject
    {
        #region Events

        event EventHandler Completed;

        #endregion Events

        #region Properties

        IArray<IAlbum> Albums
        {
            get;
        }

        IArtist Artist
        {
            get;
        }

        string Biography
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

        IArray<IImage> Portraits
        {
            get;
        }

        IArray<IArtist> SimilarArtists
        {
            get;
        }

        IArray<ITrack> Tracks
        {
            get;
        }

        IArray<ITrack> TopHitTracks
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