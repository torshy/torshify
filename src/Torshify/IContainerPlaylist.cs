namespace Torshify
{
    public interface IContainerPlaylist : IPlaylist
    {
        PlaylistType Type { get; }
    }
}
