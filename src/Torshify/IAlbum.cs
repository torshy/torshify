namespace Torshify
{
    public interface IAlbum : ISessionObject
    {
        bool IsLoaded { get; }
        IArtist Artist { get; }
        string CoverId { get; }
        bool IsAvailable { get; }
        string Name { get; }
        AlbumType Type { get; }
        int Year { get; }
    }
}