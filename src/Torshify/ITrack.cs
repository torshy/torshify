using System;

namespace Torshify
{
    public interface ITrack : ISessionObject
    {
        IAlbum Album { get; }
        IArray<IArtist> Artists { get; }
        int Disc { get; }
        TimeSpan Duration { get; }
        Error Error { get; }
        int Index { get; }
        bool IsAvailable { get; }
        bool IsStarred { get; set; }
        string Name { get; }
        int Popularity { get; }
        bool IsLoaded { get; }
    }
}