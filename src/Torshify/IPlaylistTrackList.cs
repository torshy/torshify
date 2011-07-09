namespace Torshify
{
    public interface IPlaylistTrackList : IEditableArray<IPlaylistTrack>
    {
        #region Methods

        void Add(ITrack item);

        void Add(int index, ITrack item);

        void Move(int[] indices, int newIndex);

        #endregion Methods
    }
}