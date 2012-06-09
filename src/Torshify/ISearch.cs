using System;

namespace Torshify
{
    public interface ISearch : ISessionObject
    {
        #region Events

        event EventHandler<SearchEventArgs> Completed;

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

        IArray<IPlaylistSearchResult> Playlists
        {
            get;
        }

        string DidYouMean
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

        string Query
        {
            get;
        }

        int TotalAlbums
        {
            get;
        }

        int TotalArtists
        {
            get;
        }

        int TotalTracks
        {
            get;
        }

        int TotalPlaylists
        {
            get;
        }

        IArray<ITrack> Tracks
        {
            get;
        }

        #endregion Properties
    }
}