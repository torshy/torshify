namespace Torshify
{
    public interface IArtist : ISessionObject
    {
        string Name { get; }
        bool IsLoaded { get; }
    }
}