using System;

namespace Torshify
{
    public interface IArtistBrowse : IDisposable
    {
        bool IsLoaded { get; }
        Error Error { get; }
        IArtist Artist { get; }
        IArray<IImage> Portraits { get; }
        IArray<ITrack> Tracks { get; }
        IArray<IAlbum> Albums { get; }
        IArray<IArtist> SimilarArtists { get; }
        string Biography { get; }
        bool IsComplete { get; }
        event EventHandler Completed;
    }
}