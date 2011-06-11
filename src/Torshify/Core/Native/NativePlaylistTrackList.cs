using System;

namespace Torshify.Core.Native
{
    internal class NativePlaylistTrackList : DelegateList<IPlaylistTrack>, IPlaylistTrackList
    {
        #region Fields

        private readonly Action<ITrack, int> _addTrackFunc;

        #endregion Fields

        #region Constructors

        public NativePlaylistTrackList(
            Func<int> getLength,
            Func<int, IPlaylistTrack> getIndex,
            Action<IPlaylistTrack, int> addFunc,
            Action<ITrack, int> addTrackFunc,
            Action<int> removeFunc,
            Func<bool> readonlyFunc)
            : base(getLength, getIndex, addFunc, removeFunc, readonlyFunc)
        {
            _addTrackFunc = addTrackFunc;
        }

        #endregion Constructors

        #region Public Methods

        public void Add(ITrack item)
        {
            _addTrackFunc(item, GetLength());
        }

        public void Add(int index, ITrack item)
        {
            _addTrackFunc(item, index);
        }

        #endregion Public Methods
    }
}