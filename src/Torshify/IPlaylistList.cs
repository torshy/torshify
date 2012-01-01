namespace Torshify
{
    public interface IPlaylistList : IEditableArray<IContainerPlaylist>
    {
        #region Properties

        IContainerPlaylist this[string name]
        {
            get;
        }

        #endregion Properties

        #region Methods

        IContainerPlaylist Add(string name);

        IContainerPlaylist Add(ILink<IPlaylist> playlist);

        Error AddFolder(int index, string name);

        Error RemoveAt(int index);

        #endregion Methods
    }
}