namespace Torshify
{
    public interface IPlaylistTrackList : IEditableArray<IPlaylistTrack>
    {
        #region Methods

        void Add(ITrack item);

        void Add(int index, ITrack item);

        #endregion Methods
    }
}