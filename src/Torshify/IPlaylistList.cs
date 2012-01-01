namespace Torshify
{
    public interface IPlaylistList : IEditableArray<IContainerPlaylist>
    {
        #region Methods

        IPlaylist Add(string name);

        IPlaylist Add(ILink<IPlaylist> playlist);

        Error AddFolder(int index, string name);

        Error RemoveAt(int index);

        #endregion Methods
    }
}