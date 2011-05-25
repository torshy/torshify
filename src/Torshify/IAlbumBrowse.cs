using System;

namespace Torshify
{
    public interface IAlbumBrowse : ISessionObject
    {
        IAlbum Album { get; }
        IArtist Artist { get; }
        IArray<ITrack> Tracks { get; }
        IArray<string> Copyrights { get; }
        Error Error { get; }
        string Review { get; }
        bool IsLoaded { get; }
        bool IsComplete { get; }
        event EventHandler<UserDataEventArgs> Completed;
    }
}