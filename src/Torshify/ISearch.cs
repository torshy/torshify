using System;

namespace Torshify
{
    public interface ISearch : ISessionObject
    {
        IArray<IAlbum> Albums { get; }
        IArray<IArtist> Artists { get; }
        event EventHandler<SearchEventArgs> Completed;
        string DidYouMean { get; }
        Error Error { get; }
        string Query { get; }
        int TotalAlbums { get; }
        int TotalArtists { get; }
        int TotalTracks { get; }
        bool IsComplete { get; }
        IArray<ITrack> Tracks { get; }
    }
}
