using System;

namespace Torshify
{
    public interface IToplistBrowse
    {
        bool IsLoaded { get; }
        Error Error { get; }

        IArray<IArtist> Artists { get; }
        IArray<IAlbum> Albums { get; }
        IArray<ITrack> Tracks { get; }

        bool IsComplete { get; }
        event EventHandler<UserDataEventArgs> Completed;
    }
}